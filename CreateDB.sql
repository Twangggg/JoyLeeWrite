SET NOCOUNT ON;
GO
CREATE DATABASE JoyLeeWrite;
Go
USE JoyLeeWrite;
Go

DROP TABLE IF EXISTS dbo.SeriesCategories;
DROP TABLE IF EXISTS dbo.Categories;
DROP TABLE IF EXISTS dbo.AIInteractions;
DROP TABLE IF EXISTS dbo.AutoSaves;
DROP TABLE IF EXISTS dbo.Chapters;
DROP TABLE IF EXISTS dbo.Series;
DROP TABLE IF EXISTS dbo.UserSettings;
DROP TABLE IF EXISTS dbo.Users;
GO

CREATE TABLE dbo.Users (
    UserId INT IDENTITY(1,1) PRIMARY KEY,
    Username NVARCHAR(100) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(256) NOT NULL,
    FullName NVARCHAR(200) NULL,
    Email NVARCHAR(256) NULL UNIQUE,
    AvatarUrl NVARCHAR(1000) NULL,
    Bio NVARCHAR(MAX) NULL,
    Role NVARCHAR(50) NOT NULL DEFAULT('AUTHOR'),
    CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    LastLogin DATETIME2 NULL
);
GO

CREATE TABLE dbo.Series (
    SeriesId INT IDENTITY(1,1) PRIMARY KEY,
    AuthorId INT NOT NULL,
    Title NVARCHAR(400) NOT NULL,
    Description NVARCHAR(MAX) NULL,
    CoverImgUrl NVARCHAR(1000) NULL,
    Tags NVARCHAR(500) NULL,
    WordCount INT NOT NULL DEFAULT 0,
    Status NVARCHAR(20) NOT NULL DEFAULT('draft'),
    CreatedDate DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    UpdatedDate DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    LastModified DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    CONSTRAINT FK_Series_Author FOREIGN KEY (AuthorId)
        REFERENCES dbo.Users(UserId) ON DELETE CASCADE,
    CONSTRAINT CHK_Series_Status CHECK (Status IN ('draft','published','archived'))
);
GO

CREATE TABLE dbo.Chapters (
    ChapterId INT IDENTITY(1,1) PRIMARY KEY,
    SeriesId INT NOT NULL,
    ChapterNumber INT NOT NULL,
    Title NVARCHAR(400) NULL,
    Content NVARCHAR(MAX) NULL,
    WordCount INT NOT NULL DEFAULT 0,
    Status NVARCHAR(20) NOT NULL DEFAULT('draft'),
    CreatedDate DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    LastModified DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    CONSTRAINT FK_Chapters_Series FOREIGN KEY (SeriesId)
        REFERENCES dbo.Series(SeriesId) ON DELETE CASCADE,
    CONSTRAINT CHK_Chapters_Status CHECK (Status IN ('draft','published'))
);
GO

CREATE UNIQUE INDEX UX_Chapters_Series_ChapterNumber 
ON dbo.Chapters(SeriesId, ChapterNumber);
GO

CREATE TABLE dbo.AutoSaves (
    AutoSaveId INT IDENTITY(1,1) PRIMARY KEY,
    ChapterId INT NOT NULL,
    Content NVARCHAR(MAX) NULL,
    SavedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    CONSTRAINT FK_AutoSaves_Chapter FOREIGN KEY (ChapterId)
        REFERENCES dbo.Chapters(ChapterId) ON DELETE CASCADE
);
GO

CREATE TABLE dbo.AIInteractions (
    InteractionId BIGINT IDENTITY(1,1) PRIMARY KEY,
    ChapterId INT NULL,
    Prompt NVARCHAR(MAX) NULL,
    Response NVARCHAR(MAX) NULL,
    ModelUsed NVARCHAR(200) NULL,
    Role NVARCHAR(50) NULL,
    Timestamp DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    CONSTRAINT FK_AIInteractions_Chapter FOREIGN KEY (ChapterId)
        REFERENCES dbo.Chapters(ChapterId) ON DELETE CASCADE
);
GO

CREATE TABLE dbo.Categories (
    CategoryId INT IDENTITY(1,1) PRIMARY KEY,
    CategoryName NVARCHAR(100) NOT NULL UNIQUE
);
GO

CREATE TABLE dbo.SeriesCategories (
    SeriesId INT NOT NULL,
    CategoryId INT NOT NULL,
    PRIMARY KEY (SeriesId, CategoryId),
    CONSTRAINT FK_SC_Series FOREIGN KEY (SeriesId) REFERENCES dbo.Series(SeriesId) ON DELETE CASCADE,
    CONSTRAINT FK_SC_Category FOREIGN KEY (CategoryId) REFERENCES dbo.Categories(CategoryId) ON DELETE CASCADE
);
GO

CREATE TABLE dbo.UserSettings (
    UserId INT NOT NULL,
    SettingKey NVARCHAR(200) NOT NULL,
    SettingValue NVARCHAR(MAX) NULL,
    PRIMARY KEY (UserId, SettingKey),
    CONSTRAINT FK_UserSettings_User FOREIGN KEY (UserId) REFERENCES dbo.Users(UserId) ON DELETE CASCADE
);
GO
CREATE TABLE dbo.WritingStatistics (
    StatId BIGINT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NOT NULL,
    RecordDate DATE NOT NULL,
    WordsWritten INT NOT NULL DEFAULT 0,
    ChaptersCreated INT NOT NULL DEFAULT 0,
    ChaptersModified INT NOT NULL DEFAULT 0,
    WritingTimeMinutes INT NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    
    CONSTRAINT FK_WritingStats_User FOREIGN KEY (UserId)
        REFERENCES dbo.Users(UserId) ON DELETE CASCADE,
    
    -- Đảm bảo mỗi user chỉ có 1 record cho mỗi ngày
    CONSTRAINT UX_WritingStats_UserDate UNIQUE (UserId, RecordDate)
);
GO

-- Index để truy vấn nhanh theo user và khoảng thời gian
CREATE INDEX IX_WritingStats_UserDate 
ON dbo.WritingStatistics(UserId, RecordDate DESC);
GO

CREATE TRIGGER trg_UpdateWritingStats_AfterChapterUpdate
ON dbo.Chapters
AFTER INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @UserId INT, @WordsAdded INT, @IsNew BIT;
    DECLARE @Today DATE = CAST(GETDATE() AS DATE);
    
    -- Lấy thông tin từ inserted và deleted
    SELECT 
        @UserId = s.AuthorId,
        @WordsAdded = i.WordCount - ISNULL(d.WordCount, 0),
        @IsNew = CASE WHEN d.ChapterId IS NULL THEN 1 ELSE 0 END
    FROM inserted i
    INNER JOIN dbo.Series s ON i.SeriesId = s.SeriesId
    LEFT JOIN deleted d ON i.ChapterId = d.ChapterId;
    
    -- Cập nhật thống kê
    MERGE dbo.WritingStatistics AS target
    USING (SELECT @UserId AS UserId, @Today AS RecordDate) AS source
    ON target.UserId = source.UserId AND target.RecordDate = source.RecordDate
    WHEN MATCHED THEN
        UPDATE SET 
            WordsWritten = WordsWritten + @WordsAdded,
            ChaptersCreated = ChaptersCreated + @IsNew,
            ChaptersModified = ChaptersModified + (1 - @IsNew)
    WHEN NOT MATCHED THEN
        INSERT (UserId, RecordDate, WordsWritten, ChaptersCreated, ChaptersModified)
        VALUES (@UserId, @Today, 
                CASE WHEN @WordsAdded > 0 THEN @WordsAdded ELSE 0 END,
                @IsNew, 1 - @IsNew);
END;
GO
IF OBJECT_ID('dbo.trg_Chapters_UpdateWordCount', 'TR') IS NOT NULL
    DROP TRIGGER dbo.trg_Chapters_UpdateWordCount;
GO

CREATE TRIGGER dbo.trg_Chapters_UpdateWordCount
ON dbo.Chapters
AFTER INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    
    UPDATE c
    SET 
        c.WordCount = 
            CASE 
                WHEN i.Content IS NULL OR LTRIM(RTRIM(i.Content)) = '' THEN 0
                ELSE 
                    LEN(LTRIM(RTRIM(i.Content))) 
                    - LEN(REPLACE(LTRIM(RTRIM(i.Content)), ' ', '')) 
                    + 1
            END,
        c.LastModified = SYSUTCDATETIME()
    FROM dbo.Chapters c
    JOIN inserted i ON c.ChapterId = i.ChapterId;

    UPDATE s
    SET 
        s.WordCount = (
            SELECT SUM(WordCount) FROM dbo.Chapters ch WHERE ch.SeriesId = s.SeriesId
        ),
        s.UpdatedDate = SYSUTCDATETIME(),
        s.LastModified = SYSUTCDATETIME()
    FROM dbo.Series s
    WHERE s.SeriesId IN (SELECT DISTINCT SeriesId FROM inserted);
END;
GO



IF OBJECT_ID('dbo.trg_Chapters_DeleteWordCount', 'TR') IS NOT NULL
    DROP TRIGGER dbo.trg_Chapters_DeleteWordCount;
GO

CREATE TRIGGER dbo.trg_Chapters_DeleteWordCount
ON dbo.Chapters
AFTER DELETE
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE s
    SET 
        s.WordCount = ISNULL((
            SELECT SUM(WordCount) FROM dbo.Chapters ch WHERE ch.SeriesId = s.SeriesId
        ), 0),
        s.UpdatedDate = SYSUTCDATETIME(),
        s.LastModified = SYSUTCDATETIME()
    FROM dbo.Series s
    WHERE s.SeriesId IN (SELECT DISTINCT SeriesId FROM deleted);
END;
GO


IF OBJECT_ID('dbo.trg_Series_UpdateTimestamp', 'TR') IS NOT NULL
    DROP TRIGGER dbo.trg_Series_UpdateTimestamp;
GO

CREATE TRIGGER dbo.trg_Series_UpdateTimestamp
ON dbo.Series
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE s
    SET 
        s.UpdatedDate = SYSUTCDATETIME(),
        s.LastModified = SYSUTCDATETIME()
    FROM dbo.Series s
    JOIN inserted i ON s.SeriesId = i.SeriesId;
END;
GO

IF OBJECT_ID('dbo.trg_AIInteractions_SetTimestamp', 'TR') IS NOT NULL
    DROP TRIGGER dbo.trg_AIInteractions_SetTimestamp;
GO

CREATE TRIGGER dbo.trg_AIInteractions_SetTimestamp
ON dbo.AIInteractions
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE ai
    SET ai.Timestamp = COALESCE(ai.Timestamp, SYSUTCDATETIME())
    FROM dbo.AIInteractions ai
    JOIN inserted i ON ai.InteractionId = i.InteractionId;
END;
GO

PRINT '✅ Database schema created successfully (no functions, only triggers).';
GO

INSERT INTO dbo.Users (Username, PasswordHash, FullName, Email, Bio, Role)
VALUES 
(N'anhnguyen', N'hashed_pw_1', N'Nguyễn Anh', N'anh@example.com', N'Tác giả chuyên viết truyện ngắn giả tưởng.', 'AUTHOR'),
(N'truongphu', N'hashed_pw_2', N'Trương Phú', N'phu@example.com', N'Tác giả trẻ với phong cách trinh thám - kinh dị.', 'AUTHOR'),
(N'admin01', N'admin_hashed_pw', N'Admin Hệ Thống', N'admin@example.com', N'Quản trị viên hệ thống', 'ADMIN');

INSERT INTO dbo.Categories (CategoryName)
VALUES 
(N'Fantasy'),
(N'Sci-Fi'),
(N'Romance'),
(N'Mystery'),
(N'Horror'),
(N'Adventure');
GO

INSERT INTO dbo.Series (AuthorId, Title, Description, CoverImgUrl, Tags, Status)
VALUES
(1, N'Vùng Đất Bóng Đêm', N'Một thế giới nơi ánh sáng bị nuốt chửng bởi bóng tối, con người phải tìm cách sinh tồn.', 
 N'https://example.com/covers/darkland.jpg', N'fantasy,dark,magic', 'draft'),

(1, N'Nhật Ký Không Gian', N'Hành trình của một phi hành gia lạc vào chiều không gian song song.', 
 N'https://example.com/covers/space_diary.jpg', N'sci-fi,adventure,time', 'published'),

(2, N'Thành Phố Không Ngủ', N'Câu chuyện trinh thám về những vụ án bí ẩn trong thành phố.', 
 N'https://example.com/covers/city.jpg', N'mystery,detective', 'draft');
GO



INSERT INTO dbo.SeriesCategories (SeriesId, CategoryId) VALUES (1, 1), (1, 6);

INSERT INTO dbo.SeriesCategories (SeriesId, CategoryId) VALUES (2, 2);

INSERT INTO dbo.SeriesCategories (SeriesId, CategoryId) VALUES (3, 4), (3, 5);
GO


INSERT INTO dbo.Chapters (SeriesId, ChapterNumber, Title, Content, Status)
VALUES
(1, 1, N'Khởi Đầu Trong Bóng Tối', 
 N'Khi mặt trời biến mất, thế giới chìm vào bóng tối vĩnh hằng. Con người hoảng loạn tìm lối thoát.', 'published'),

(1, 2, N'Ngọn Đèn Cuối Cùng', 
 N'Một nhóm người sống sót tìm thấy ngọn đèn kỳ lạ phát sáng không bao giờ tắt.', 'draft'),

(2, 1, N'Hành Trình Bắt Đầu', 
 N'Tôi tỉnh dậy giữa không gian vô tận. Con tàu trôi dạt, và chỉ còn tôi cùng nhật ký bay.', 'published'),

(3, 1, N'Án Mạng Trong Đêm', 
 N'Một vụ án mạng không có hung khí, nạn nhân chết trong căn phòng khóa kín từ bên trong.', 'published');
GO

INSERT INTO dbo.AutoSaves (ChapterId, Content)
VALUES
(1, N'Bản nháp cũ của chương 1. Nội dung chưa hoàn chỉnh.'),
(2, N'Đang chỉnh sửa phần hội thoại ở chương 2.'),
(3, N'Phiên bản draft trước khi publish.'),
(4, N'Thử nghiệm viết lại đoạn mở đầu.');
GO

-- ========== 7. AIINTERACTIONS ==========
INSERT INTO dbo.AIInteractions (ChapterId, Prompt, Response, ModelUsed, Role)
VALUES
(1, N'Hãy gợi ý tiêu đề hấp dẫn hơn cho chương này.', N'“Khi Ánh Sáng Tắt Lịm”', N'gpt-4', 'assistant'),
(2, N'Hãy viết lại đoạn này cho tự nhiên hơn.', N'“Nhóm sống sót nhìn nhau, niềm hy vọng nhỏ bé còn sót lại...”', N'gpt-4', 'assistant'),
(3, N'Hãy tóm tắt nội dung chương 1.', N'Một phi hành gia cô độc thức dậy trong không gian vô tận, bắt đầu hành trình tìm về nhà.', N'gpt-4', 'assistant');
GO

INSERT INTO dbo.UserSettings (UserId, SettingKey, SettingValue)
VALUES
(1, 'theme', 'dark'),
(1, 'autosave_interval', '60'),
(2, 'theme', 'light'),
(3, 'language', 'vi');
GO
INSERT INTO dbo.WritingStatistics (UserId, RecordDate, WordsWritten, ChaptersCreated, ChaptersModified, WritingTimeMinutes)
VALUES
(4, DATEADD(DAY, -4, CAST(GETDATE() AS DATE)), 10, 1, 0, 30),
(4, DATEADD(DAY, -3, CAST(GETDATE() AS DATE)), 20, 1, 1, 40),
(4, DATEADD(DAY, -2, CAST(GETDATE() AS DATE)), 50, 0, 2, 25),
(4, DATEADD(DAY, -1, CAST(GETDATE() AS DATE)), 25, 1, 1, 55),
(4, CAST(GETDATE() AS DATE), 30, 1, 0, 60);
