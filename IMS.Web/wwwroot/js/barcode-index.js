<script type="text/javascript">
    $(document).ready(function() {
        initializeComponents();
    loadDynamicOptions();
        });

    function initializeComponents() {
            // Initialize DataTable only if there are rows
            if ($('#barcodesTable tbody tr').length > 0) {
        $('#barcodesTable').DataTable({
            responsive: true,
            pageLength: 25,
            order: [[5, "desc"]],
            columnDefs: [
                { targets: [-1], orderable: false }
            ]
        });
            }
        }

    function loadDynamicOptions() {
        // Load Stores
        $.get('@Url.Action("GetStores", "Barcode")')
            .done(function (data) {
                var storeSelect = $('#storeId');
                storeSelect.empty().append('<option value="">All Stores</option>');
                $.each(data, function (index, item) {
                    storeSelect.append('<option value="' + item.value + '">' + item.text + '</option>');
                });
            })
            .fail(function () {
                console.log('Failed to load stores');
            });

    // Load Categories
    $.get('@Url.Action("GetCategories", "Barcode")')
    .done(function(data) {
                    var categorySelect = $('#categoryId');
    categorySelect.empty().append('<option value="">All Categories</option>');
    $.each(data, function(index, item) {
        categorySelect.append('<option value="' + item.value + '">' + item.text + '</option>');
                    });
                })
    .fail(function() {
        console.log('Failed to load categories');
                });

    // Load Locations
    $.get('@Url.Action("GetLocations", "Barcode")')
    .done(function(data) {
                    var locationSelect = $('#location');
    locationSelect.empty().append('<option value="">All Locations</option>');
    $.each(data, function(index, item) {
        locationSelect.append('<option value="' + item.value + '">' + item.text + '</option>');
                    });
                })
    .fail(function() {
        console.log('Failed to load locations');
                });
        }

    function clearFilters() {
        document.getElementById('filterForm').reset();
    document.getElementById('filterForm').submit();
        }

    function filterByRecentScans() {
        document.getElementById('scanStatus').value = 'recent';
    document.getElementById('filterForm').submit();
        }

    function filterByMostPrinted() {
        window.location.href = '@Url.Action("Index", "Barcode")?sortBy=printed&order=desc';
        }

    function filterByUnscanned() {
        document.getElementById('scanStatus').value = 'unscanned';
    document.getElementById('filterForm').submit();
        }

    function showQRCode(barcodeNumber, barcodeId) {
            var qrUrl = '@Url.Action("GenerateQRImage", "Barcode")?text=' + encodeURIComponent(barcodeNumber);
    document.getElementById('qrCodeImage').src = qrUrl;
    document.getElementById('modalBarcodeNumber').textContent = barcodeNumber;

    // Load additional barcode details
    $.get('@Url.Action("GetBarcodeDetails", "Barcode")', {id: barcodeId })
    .done(function(data) {
                    if (data.success) {
        document.getElementById('modalBarcodeType').textContent = data.barcodeType || 'CODE128';
    document.getElementById('modalGeneratedDate').textContent = new Date(data.generatedDate).toLocaleDateString();
    document.getElementById('modalPrintCount').textContent = data.printCount || 0;
    document.getElementById('modalScanCount').textContent = data.scanCount || 0;
                    }
                })
    .fail(function() {
        console.log('Failed to load barcode details');
                });

    $('#qrCodeModal').modal('show');
        }

    function recordScan(barcodeId) {
        document.getElementById('scanBarcodeId').value = barcodeId;
    document.getElementById('scanForm').reset();
    $('#scanModal').modal('show');
        }

    function submitScan() {
            var barcodeId = document.getElementById('scanBarcodeId').value;
    var location = document.getElementById('scanLocation').value;
    var action = document.getElementById('scanAction').value;
    var notes = document.getElementById('scanNotes').value;

    if (!location) {
        alert('Please enter a scan location');
    return;
            }

    var data = {
        barcodeId: barcodeId,
    location: location,
    action: action,
    notes: notes
            };

    $.post('@Url.Action("RecordScan", "Barcode")', data)
    .done(function(response) {
                    if (response.success) {
        alert('Scan recorded successfully');
    $('#scanModal').modal('hide');
    location.reload();
                    } else {
        alert(response.message || 'Failed to record scan');
                    }
                })
    .fail(function() {
        alert('Error recording scan');
                });
        }

    function getFormData() {
            return $('#filterForm').serialize();
        }

    function exportToExcel() {
            var currentFilters = getFormData();
    var url = '@Url.Action("ExportToExcel", "Barcode")?' + currentFilters;
    window.open(url, '_blank');
        }

    function exportToPDF() {
            var currentFilters = getFormData();
    var url = '@Url.Action("ExportToPDF", "Barcode")?' + currentFilters;
    window.open(url, '_blank');
        }

    function exportToCSV() {
            var currentFilters = getFormData();
    var url = '@Url.Action("ExportToCSV", "Barcode")?' + currentFilters;
    window.open(url, '_blank');
        }

    function downloadQR() {
            var canvas = document.createElement('canvas');
    var ctx = canvas.getContext('2d');
    var img = document.getElementById('qrCodeImage');

    canvas.width = img.naturalWidth;
    canvas.height = img.naturalHeight;
    ctx.drawImage(img, 0, 0);

    var link = document.createElement('a');
    link.download = document.getElementById('modalBarcodeNumber').textContent + '_QR.png';
    link.href = canvas.toDataURL();
    link.click();
        }

    function printQR() {
            var printWindow = window.open('', '', 'height=600,width=800');
    var qrImage = document.getElementById('qrCodeImage').src;
    var barcodeNumber = document.getElementById('modalBarcodeNumber').textContent;

    var printContent = '<!DOCTYPE html><html><head><title>QR Code - ' + barcodeNumber + '</title>';
        printContent += '<style>body{text - align:center;font-family:Arial;padding:20px;}h2{margin - bottom:20px;}img{max - width:400px;border:2px solid #007bff;border-radius:10px;}</style>';
        printContent += '</head><body>';
            printContent += '<h2>' + barcodeNumber + '</h2>';
            printContent += '<img src="' + qrImage + '" />';
            printContent += '<p style="margin-top:20px;color:#666;">Generated on ' + new Date().toLocaleDateString() + '</p>';
            printContent += '</body></html>';

    printWindow.document.write(printContent);
    printWindow.document.close();
    printWindow.print();
        }

    function shareQR() {
            var barcodeNumber = document.getElementById('modalBarcodeNumber').textContent;

    if (navigator.share) {
        navigator.share({
            title: 'QR Code - ' + barcodeNumber,
            text: 'Barcode: ' + barcodeNumber,
            url: window.location.href
        });
            } else {
        // Fallback: copy to clipboard
        navigator.clipboard.writeText(barcodeNumber).then(function () {
            alert('Barcode number copied to clipboard');
        }).catch(function () {
            // Fallback for older browsers
            var textArea = document.createElement('textarea');
            textArea.value = barcodeNumber;
            document.body.appendChild(textArea);
            textArea.select();
            document.execCommand('copy');
            document.body.removeChild(textArea);
            alert('Barcode number copied to clipboard');
        });
            }
        }

    function saveFilterPreset() {
        alert('Filter preset functionality coming soon');
        }
</script>