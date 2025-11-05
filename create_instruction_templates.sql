-- Create Instruction Templates for AllotmentLetter
USE ansvdp_ims;
GO

-- Create table for instruction templates
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'AllotmentLetterTemplates')
BEGIN
    CREATE TABLE AllotmentLetterTemplates (
        Id INT PRIMARY KEY IDENTITY(1,1),
        TemplateName NVARCHAR(200) NOT NULL,
        TemplateNameBn NVARCHAR(200) NOT NULL,
        Category NVARCHAR(100) NOT NULL, -- 'Computer', 'Uniform', 'Equipment', 'General'

        -- Opening paragraph
        OpeningText NVARCHAR(MAX),
        OpeningTextBn NVARCHAR(MAX),

        -- Instruction Paragraphs
        Instruction1 NVARCHAR(MAX), -- Committee & Collection
        Instruction1Bn NVARCHAR(MAX),

        Instruction2 NVARCHAR(MAX), -- Warranty/Transport
        Instruction2Bn NVARCHAR(MAX),

        Instruction3 NVARCHAR(MAX), -- Serial/Model verification
        Instruction3Bn NVARCHAR(MAX),

        Instruction4 NVARCHAR(MAX), -- Quality check
        Instruction4Bn NVARCHAR(MAX),

        Instruction5 NVARCHAR(MAX), -- Old equipment disposal
        Instruction5Bn NVARCHAR(MAX),

        -- Distribution list (comma separated)
        DistributionList NVARCHAR(MAX),
        DistributionListBn NVARCHAR(MAX),

        -- Attachment text
        AttachmentText NVARCHAR(200),
        AttachmentTextBn NVARCHAR(200),

        -- Audit fields
        IsActive BIT NOT NULL DEFAULT 1,
        CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
        CreatedBy NVARCHAR(100),
        UpdatedAt DATETIME,
        UpdatedBy NVARCHAR(100)
    );

    PRINT 'Table AllotmentLetterTemplates created successfully!';
END
ELSE
BEGIN
    PRINT 'Table AllotmentLetterTemplates already exists.';
END
GO

-- Insert Computer Equipment Template (from Allot-428.pdf)
INSERT INTO AllotmentLetterTemplates (
    TemplateName, TemplateNameBn, Category,
    OpeningTextBn,
    Instruction1Bn, Instruction2Bn, Instruction3Bn, Instruction4Bn, Instruction5Bn,
    DistributionListBn,
    AttachmentTextBn,
    CreatedBy, IsActive
) VALUES (
    'Computer Equipment Allotment',
    'কম্পিউটার সরঞ্জাম বরাদ্দ',
    'Computer',

    -- Opening
    'উপরোক্ত বিষয়ের প্রেক্ষিতে বিভিন্ন দপ্তর/ইউনিটের চাহিদার আলোকে আয়ুষ্কাল বিবেচনায় রোডপত্র-''ক'' অনুযায়ী ডেস্কটপ কম্পিউটার সেট ও কম্পিউটার এক্সেসরিজ বরাদ্দ প্রদান করা হলো।',

    -- Instruction 1
    '২। উপমহাপরিচালক/পরিচালক রেঞ্জ কর্তৃক গঠিত কমিটিকে ক্ষমতাপত্র দিয়ে আগামী [তারিখ] তারিখের মধ্যে অবশ্যই ভাউচার গ্রহণ পূর্বক একাডেমিস্থ কেন্দ্রীয় আনসার/ভিডিপি ভান্ডার হতে বরাদ্দকৃত উপকরণ গ্রহণ করবেন। গ্রহণকারী নিজ নামীয় সীল সঙ্গে আনবেন।',

    -- Instruction 2
    '৩। বরাদ্দকৃত ডেস্কটপ কম্পিউটার সেট ও কম্পিউটার এক্সেসরিজ এর ওয়ারেন্টি থাকা সাপেক্ষে সরবরাহকারী প্রতিষ্ঠান উক্ত সময়ে ফ্রি সার্ভিসিং প্রদান করবেন বিধায় বরাদ্দকৃত উপকরণে কোন সমস্যা দেখা দিলে ওয়ারেন্টি কার্ডসহ সদর দপ্তর প্রভিশন শাখায় যোগাযোগ করার জন্য অনুরোধ করা হলো।',

    -- Instruction 3
    '৪। ভাউচারে বরাদ্দকৃত উপকরণের মডেল নম্বর ও সিরিয়াল নম্বর উল্লেখ থাকবে। উক্ত উপকরণ গ্রহণের সময় সিরিয়াল নম্বর মিলিয়ে নেয়ার জন্য অনুরোধ করা হলো। ভবিষ্যতে উপকরণ ফেরত/হস্তান্তর/অকেজোকরণ করা হলে উক্ত সিরিয়াল ও মডেল নম্বর উল্লেখ থাকতে হবে।',

    -- Instruction 4 (empty for computer template)
    NULL,

    -- Instruction 5
    '৫। উল্লেখ্য, পুরাতন ডেস্কটপ কম্পিউটার সেট ও কম্পিউটার এক্সেসরিজ ব্যবহার্য অনুপযোগী/অকেজোকরণের উপযোগী হলে সেগুলোর মডেল নম্বর ও সিরিয়াল নম্বর উল্লেখ পূর্বক কেন্দ্রীয় ভান্ডার, আনসার-ভিডিপি একাডেমীতে জমা করার নির্দেশনা প্রদান করা হলো।',

    -- Distribution List
    '১। মহাপরিচালক মহোদয়ের সচিবালয়, বাংলাদেশ আনসার ও গ্রাম প্রতিরক্ষা বাহিনী, সদর দপ্তর, খিলগাঁও, ঢাকা।
২। অতিরিক্ত মহাপরিচালক মহোদয়ের দপ্তর, বাংলাদেশ আনসার ও গ্রাম প্রতিরক্ষা বাহিনী, সদর দপ্তর, খিলগাঁও, ঢাকা। সময় অবগতির জন্য।
৩। কমান্ডান্ট, বাংলাদেশ আনসার-ভিডিপি একাডেমী, সফিপুর, গাজীপুর।',

    'সংযুক্ত-রোডপত্র "ক" ০১ (এক) পাতা।',

    'system',
    1
);

-- Insert Uniform/Kit Template (from Allot.-নমুনা.pdf)
INSERT INTO AllotmentLetterTemplates (
    TemplateName, TemplateNameBn, Category,
    OpeningTextBn,
    Instruction1Bn, Instruction2Bn, Instruction3Bn, Instruction4Bn, Instruction5Bn,
    DistributionListBn,
    AttachmentTextBn,
    CreatedBy, IsActive
) VALUES (
    'Uniform/Kit Allotment',
    'ইউনিফর্ম/কিট বরাদ্দ',
    'Uniform',

    -- Opening
    'উপরোক্ত বিষয়ের প্রেক্ষিতে সময় অবগতি ও প্রয়োজনীয় ব্যবস্থা গ্রহণের জন্য জানানো যাচ্ছে যে, আয়ুস্কাল বিবেচনায় প্রাপ্যতার ৫০% হিসাবে রোডপত্র-"ক" অনুযায়ী কিট ব্যাগ (নতুন নকশা) বরাদ্দ দেয়া হলো।',

    -- Instruction 1
    '২। উপমহাপরিচালক/পরিচালক রেঞ্জ কর্তৃক গঠিত কমিটিকে ক্ষমতাপত্র দিয়ে আগামী [তারিখ] তারিখের মধ্যে অবশ্যই ভাউচার গ্রহণ পূর্বক একাডেমিস্থ কেন্দ্রীয় আনসার/ভিডিপি ভান্ডার হতে বরাদ্দকৃত উপকরণ গ্রহণ করবেন। বরাদ্দকৃত উপকরণ পরিবহনের সময় স্কটের প্রয়োজন হলে স্কট নিয়ে আসবেন। গ্রহণকারী নিজ নামীয় সীল সঙ্গে আনবেন।',

    -- Instruction 2
    '৩। বরাদ্দকৃত উপকরণ পরিবহনের ক্ষেত্রে রেঞ্জের গাড়ী ব্যবহার করার জন্য অনুরোধ করা হলো। রেঞ্জের পরিবহন ব্যতীত অতিরিক্ত যানবাহন প্রয়োজন হলে প্রশাসন-উইং শাখায় যোগাযোগ করার জন্য অনুরোধ করা হলো।',

    -- Instruction 3
    '৪। বরাদ্দকৃত উপকরণের জামানত অবমুক্ত করার নিমিত্তে কেন্দ্রীয় আনসার/ভিডিপি ভাণ্ডার হতে উপকরণ গ্রহণের ৩০(ত্রিশ) দিনের মধ্যে এর গুনগতমান (আপত্তি/অনাপত্তি/ত্রুটি) যাচাই করে প্রতিবেদন (লেজারভুক্তির প্রত্যয়ণসহ) ভাণ্ডার শাখায় প্রেরণ করার জন্য অনুরোধ করা হলো।',

    -- Instruction 4 (empty)
    NULL,

    -- Instruction 5
    'উল্লেখ্য যে, বরাদ্দকৃত উপকরণ গ্রহণ ও বিতরণ পূর্বক পুরাতন উপকরণ অকেজোকরণ করে সমন্বয় করার জন্য অনুরোধ করা হলো।',

    -- Distribution List
    '১। মহাপরিচালক মহোদয়ের সচিবালয়, বাংলাদেশ আনসার ও গ্রাম প্রতিরক্ষা বাহিনী, সদর দপ্তর, খিলগাঁও, ঢাকা।
২। অতিরিক্ত মহাপরিচালক মহোদয়ের দপ্তর, বাংলাদেশ আনসার ও গ্রাম প্রতিরক্ষা বাহিনী, সদর দপ্তর, খিলগাঁও, ঢাকা। সময় অবগতির জন্য।
৩। কমান্ডযান্ট, বাংলাদেশ আনসার-ভিডিপি একাডেমী, সফিপুর, গাজীপুর।
৪। উপমহাপরিচালক (প্রশাসন/অপারেশনস্/প্রশিক্ষণ), বাংলাদেশ আনসার ও গ্রাম প্রতিরক্ষা বাহিনী, সদর দপ্তর, খিলগাঁও, ঢাকা।',

    'সংযুক্ত-রোডপত্র-০১ (এক) পাতা',

    'system',
    1
);

-- Insert General Template
INSERT INTO AllotmentLetterTemplates (
    TemplateName, TemplateNameBn, Category,
    OpeningTextBn,
    Instruction1Bn, Instruction2Bn, Instruction3Bn,
    DistributionListBn,
    AttachmentTextBn,
    CreatedBy, IsActive
) VALUES (
    'General Allotment',
    'সাধারণ বরাদ্দ',
    'General',

    'উপরোক্ত বিষয়ের প্রেক্ষিতে সময় অবগতি ও প্রয়োজনীয় ব্যবস্থা গ্রহণের জন্য জানানো যাচ্ছে যে, রোডপত্র-"ক" অনুযায়ী বরাদ্দ প্রদান করা হলো।',

    '২। উপমহাপরিচালক/পরিচালক রেঞ্জ কর্তৃক গঠিত কমিটিকে ক্ষমতাপত্র দিয়ে নির্ধারিত সময়ের মধ্যে অবশ্যই ভাউচার গ্রহণ পূর্বক একাডেমিস্থ কেন্দ্রীয় আনসার/ভিডিপি ভান্ডার হতে বরাদ্দকৃত উপকরণ গ্রহণ করবেন। গ্রহণকারী নিজ নামীয় সীল সঙ্গে আনবেন।',

    '৩। বরাদ্দকৃত উপকরণ পরিবহনের ক্ষেত্রে রেঞ্জের গাড়ী ব্যবহার করার জন্য অনুরোধ করা হলো।',

    '৪। বরাদ্দকৃত উপকরণের জামানত অবমুক্ত করার নিমিত্তে কেন্দ্রীয় আনসার/ভিডিপি ভাণ্ডার হতে উপকরণ গ্রহণের ৩০(ত্রিশ) দিনের মধ্যে এর গুনগতমান যাচাই করে প্রতিবেদন ভাণ্ডার শাখায় প্রেরণ করার জন্য অনুরোধ করা হলো।',

    '১। মহাপরিচালক মহোদয়ের সচিবালয়, বাংলাদেশ আনসার ও গ্রাম প্রতিরক্ষা বাহিনী, সদর দপ্তর, খিলগাঁও, ঢাকা।
২। অতিরিক্ত মহাপরিচালক মহোদয়ের দপ্তর, বাংলাদেশ আনসার ও গ্রাম প্রতিরক্ষা বাহিনী, সদর দপ্তর, খিলগাঁও, ঢাকা। সময় অবগতির জন্য।
৩। কমান্ডান্ট, বাংলাদেশ আনসার-ভিডিপি একাডেমী, সফিপুর, গাজীপুর।',

    'সংযুক্ত-রোডপত্র "ক" ০১ (এক) পাতা।',

    'system',
    1
);

PRINT 'All templates inserted successfully!';

-- Verify
SELECT Id, TemplateName, TemplateNameBn, Category
FROM AllotmentLetterTemplates
WHERE IsActive = 1
ORDER BY Category, TemplateName;

GO
