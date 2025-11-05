// Offline Scanner Module for IMS
// This file should be placed in wwwroot/js/offline-scanner.js

using System;

(function() {
    'use strict';

    // Configuration
    const CONFIG = {
        dbName: 'IMSOfflineDB',
        dbVersion: 1,
        storeName: 'scannedItems',
        syncInterval: 30000, // 30 seconds
        maxRetries: 3
    }
;

// IndexedDB instance
let db = null;

// Initialize the offline scanner module
function init()
{
    initDatabase();
    setupEventListeners();
    setupPeriodicSync();

    // Check online status on load
    updateOnlineStatus();
}

// Initialize IndexedDB
function initDatabase()
{
    const request = indexedDB.open(CONFIG.dbName, CONFIG.dbVersion);

    request.onerror = function(event) {
        console.error('Database error:', event.target.error);
        showNotification('Failed to initialize offline storage', 'error');
    }
    ;

    request.onsuccess = function(event) {
        db = event.target.result;
        console.log('Offline database initialized');

        // Sync any pending items
        if (navigator.onLine)
        {
            syncOfflineScans();
        }
    }
    ;

    request.onupgradeneeded = function(event) {
        db = event.target.result;

        // Create object store for scanned items
        if (!db.objectStoreNames.contains(CONFIG.storeName))
        {
            const objectStore = db.createObjectStore(CONFIG.storeName, {
            keyPath: 'id', 
                    autoIncrement: true
                });

            // Create indexes
            objectStore.createIndex('synced', 'synced', { unique: false });
            objectStore.createIndex('timestamp', 'timestamp', { unique: false });
            objectStore.createIndex('action', 'action', { unique: false });
        }
    }
    ;
}

// Store scan data offline
async function storeOfflineScan(scanData)
{
    return new Promise((resolve, reject) => {
    const transaction = db.transaction([CONFIG.storeName], 'readwrite');
    const store = transaction.objectStore(CONFIG.storeName);

    const data = {
                ...scanData,
                timestamp: new Date().toISOString(),
                synced: false,
                retryCount: 0,
                deviceId: getDeviceId()
            };

const request = store.add(data);

request.onsuccess = function() {
    console.log('Scan stored offline:', data);
    updateOfflineCount();
    showNotification('Scan saved offline', 'info');
    resolve(request.result);
}
;

request.onerror = function() {
    console.error('Error storing scan:', request.error);
    showNotification('Failed to save scan', 'error');
    reject(request.error);
}
;
        });
    }

    // Sync offline scans when online
    async function syncOfflineScans()
{
    if (!navigator.onLine || !db)
    {
        return;
    }

    const transaction = db.transaction([CONFIG.storeName], 'readonly');
    const store = transaction.objectStore(CONFIG.storeName);
    const index = store.index('synced');
    const request = index.getAll(false);

    request.onsuccess = async function(event) {
        const unsynced = event.target.result;

        if (unsynced.length === 0)
        {
            console.log('No offline scans to sync');
            return;
        }

        console.log(`Syncing ${ unsynced.length}
        offline scans...`);
        showNotification(`Syncing ${ unsynced.length}
        offline scans...`, 'info');

        for (const scan of unsynced) {
            await syncSingleScan(scan);
        }

        updateOfflineCount();
    }
    ;
}

// Sync a single scan to the server
async function syncSingleScan(scan)
{
    try
    {
        const response = await fetch('/api/barcode/offline-sync', {
        method: 'POST',
                headers:
            {
                'Content-Type': 'application/json',
                    'X-Requested-With': 'XMLHttpRequest'
                },
                body: JSON.stringify({
barcode: scan.barcode,
                    action: scan.action,
                    quantity: scan.quantity,
                    timestamp: scan.timestamp,
                    location: scan.location,
                    notes: scan.notes,
                    deviceId: scan.deviceId
                })
            });

if (response.ok)
{
    // Mark as synced
    await markAsSynced(scan.id);
    console.log('Scan synced successfully:', scan.id);
}
else
{
    // Handle sync failure
    await handleSyncFailure(scan);
}
        } catch (error) {
    console.error('Sync error:', error);
    await handleSyncFailure(scan);
}
    }

    // Mark scan as synced
    async function markAsSynced(scanId)
{
    return new Promise((resolve, reject) => {
        const transaction = db.transaction([CONFIG.storeName], 'readwrite');
        const store = transaction.objectStore(CONFIG.storeName);
        const request = store.get(scanId);

        request.onsuccess = function(event) {
            const scan = event.target.result;
            if (scan)
            {
                scan.synced = true;
                scan.syncedAt = new Date().toISOString();

                const updateRequest = store.put(scan);
                updateRequest.onsuccess = () => resolve();
                updateRequest.onerror = () => reject(updateRequest.error);
            }
        }
        ;
    });
}

// Handle sync failure
async function handleSyncFailure(scan)
{
    const transaction = db.transaction([CONFIG.storeName], 'readwrite');
    const store = transaction.objectStore(CONFIG.storeName);
    const request = store.get(scan.id);

    request.onsuccess = function(event) {
        const data = event.target.result;
        if (data)
        {
            data.retryCount = (data.retryCount || 0) + 1;
            data.lastSyncAttempt = new Date().toISOString();

            if (data.retryCount >= CONFIG.maxRetries)
            {
                data.syncFailed = true;
                showNotification(`Failed to sync scan after ${ CONFIG.maxRetries}
                attempts`, 'error');
            }

            store.put(data);
        }
    }
    ;
}

// Setup event listeners
function setupEventListeners()
{
    // Online/Offline events
    window.addEventListener('online', handleOnline);
    window.addEventListener('offline', handleOffline);

    // Scanner integration
    document.addEventListener('barcodescanned', handleBarcodeScan);
}

// Handle online event
function handleOnline()
{
    console.log('Connection restored');
    updateOnlineStatus();
    showNotification('Connection restored - syncing data...', 'success');
    syncOfflineScans();
}

// Handle offline event
function handleOffline()
{
    console.log('Connection lost');
    updateOnlineStatus();
    showNotification('Working offline - scans will be synced when connection is restored', 'warning');
}

// Handle barcode scan event
async function handleBarcodeScan(event)
{
    const scanData = event.detail;

    if (navigator.onLine)
    {
        // Try to process online first
        try
        {
            await processScanOnline(scanData);
        }
        catch (error)
        {
            // If online processing fails, store offline
            console.error('Online processing failed, storing offline:', error);
            await storeOfflineScan(scanData);
        }
    }
    else
    {
        // Store offline immediately
        await storeOfflineScan(scanData);
    }
}

// Process scan online
async function processScanOnline(scanData)
{
    const response = await fetch('/api/barcode/scan', {
    method: 'POST',
            headers:
        {
            'Content-Type': 'application/json',
                'X-Requested-With': 'XMLHttpRequest'
            },
            body: JSON.stringify(scanData)
        });

if (!response.ok)
{
    throw new Error(`Server returned ${ response.status }`);
}

const result = await response.json();
showNotification('Scan processed successfully', 'success');
return result;
    }

    // Update offline count display
    async function updateOfflineCount()
{
    if (!db) return;

    const transaction = db.transaction([CONFIG.storeName], 'readonly');
    const store = transaction.objectStore(CONFIG.storeName);
    const index = store.index('synced');
    const request = index.count(false);

    request.onsuccess = function(event) {
        const count = event.target.result;
        const badge = document.getElementById('offline-scan-count');

        if (badge)
        {
            badge.textContent = count;
            badge.style.display = count > 0 ? 'inline-block' : 'none';
        }

        // Update any other UI elements
        const statusElements = document.querySelectorAll('[data-offline-count]');
        statusElements.forEach(el => {
            el.textContent = count;
        });
    }
    ;
}

// Update online status indicator
function updateOnlineStatus()
{
    const isOnline = navigator.onLine;
    const statusIndicator = document.getElementById('connection-status');

    if (statusIndicator)
    {
        statusIndicator.className = isOnline ? 'online' : 'offline';
        statusIndicator.title = isOnline ? 'Connected' : 'Offline';
    }

    // Update any other UI elements
    document.body.classList.toggle('offline-mode', !isOnline);
}

// Setup periodic sync
function setupPeriodicSync()
{
    // Try to sync every 30 seconds when online
    setInterval(() => {
        if (navigator.onLine)
        {
            syncOfflineScans();
        }
    }, CONFIG.syncInterval);
}

// Show notification
function showNotification(message, type = 'info')
{
    // Check if toastr is available
    if (typeof toastr !== 'undefined')
    {
        toastr[type](message);
    }
    else
    {
        console.log(`[${ type.toUpperCase()}] ${ message}`);
    }
}

// Get or create device ID
function getDeviceId()
{
    let deviceId = localStorage.getItem('ims-device-id');

    if (!deviceId)
    {
        deviceId = 'device-' + Date.now() + '-' + Math.random().toString(36).substr(2, 9);
        localStorage.setItem('ims-device-id', deviceId);
    }

    return deviceId;
}

// Public API
window.OfflineScanner = {
init: init,
        storeOfflineScan: storeOfflineScan,
        syncOfflineScans: syncOfflineScans,
        getOfflineCount: async function()
    {
        return new Promise((resolve) => {
            if (!db)
            {
                resolve(0);
                return;
            }

            const transaction = db.transaction([CONFIG.storeName], 'readonly');
            const store = transaction.objectStore(CONFIG.storeName);
            const index = store.index('synced');
            const request = index.count(false);

            request.onsuccess = (event) => resolve(event.target.result);
        request.onerror = () => resolve(0);
    });
},
        clearOfflineData: async function()
{
    return new Promise((resolve, reject) => {
        if (!db)
        {
            reject('Database not initialized');
            return;
        }

        const transaction = db.transaction([CONFIG.storeName], 'readwrite');
        const store = transaction.objectStore(CONFIG.storeName);
        const request = store.clear();

        request.onsuccess = () => {
            updateOfflineCount();
            showNotification('Offline data cleared', 'success');
            resolve();
        };

        request.onerror = () => reject(request.error);
    });
}
    };

// Initialize when DOM is ready
if (document.readyState === 'loading')
{
    document.addEventListener('DOMContentLoaded', init);
}
else
{
    init();
}
})();