CREATE TABLE Books (
    BookID INT IDENTITY(1,1) PRIMARY KEY,
    Title NVARCHAR(200) NOT NULL,
    Author NVARCHAR(150) NOT NULL,
    Genre NVARCHAR(100),
    PublishYear INT CHECK (PublishYear >= 1000 AND PublishYear <= YEAR(GETDATE())),
    Publisher NVARCHAR(150),
    PageCount INT CHECK (PageCount > 0),
    Language NVARCHAR(50),
    DateAdded DATE DEFAULT GETDATE(),
    TableOfContents XML
);

-- Хранимая процедура для получения всех книг
CREATE PROCEDURE usp_GetAllBooks
AS
BEGIN
    SELECT 
        BookID,
        Title,
        Author,
        Genre,
        PublishYear,
        Publisher,
        PageCount,
        Language,
        DateAdded,
        CAST(TableOfContents AS NVARCHAR(MAX)) AS TableOfContents
    FROM Books
    ORDER BY Title, Author
END
GO

-- Хранимая процедура для получения книги по ID
CREATE PROCEDURE usp_GetBookById
    @BookID INT
AS
BEGIN
    SELECT 
        BookID,
        Title,
        Author,
        Genre,
        PublishYear,
        Publisher,
        PageCount,
        Language,
        DateAdded,
        CAST(TableOfContents AS NVARCHAR(MAX)) AS TableOfContents
    FROM Books
    WHERE BookID = @BookID
END
GO

-- Хранимая процедура для создания новой книги
CREATE PROCEDURE usp_CreateBook
    @Title NVARCHAR(200),
    @Author NVARCHAR(150),
    @Genre NVARCHAR(100) = NULL,
    @PublishYear INT,
    @Publisher NVARCHAR(150) = NULL,
    @PageCount INT,
    @Language NVARCHAR(50) = NULL,
    @TableOfContents XML = NULL,
    @NewBookID INT OUTPUT
AS
BEGIN
    INSERT INTO Books (
        Title, 
        Author, 
        Genre, 
        PublishYear, 
        Publisher, 
        PageCount, 
        Language, 
        TableOfContents,
        DateAdded
    )
    VALUES (
        @Title,
        @Author,
        @Genre,
        @PublishYear,
        @Publisher,
        @PageCount,
        @Language,
        @TableOfContents,
        GETDATE()
    )
    
    SET @NewBookID = SCOPE_IDENTITY()
END
GO

-- Хранимая процедура для обновления книги
CREATE PROCEDURE usp_UpdateBook
    @BookID INT,
    @Title NVARCHAR(200),
    @Author NVARCHAR(150),
    @Genre NVARCHAR(100) = NULL,
    @PublishYear INT,
    @Publisher NVARCHAR(150) = NULL,
    @PageCount INT,
    @Language NVARCHAR(50) = NULL,
    @TableOfContents XML = NULL
AS
BEGIN
    UPDATE Books
    SET 
        Title = @Title,
        Author = @Author,
        Genre = @Genre,
        PublishYear = @PublishYear,
        Publisher = @Publisher,
        PageCount = @PageCount,
        Language = @Language,
        TableOfContents = @TableOfContents
    WHERE BookID = @BookID
END
GO

-- Хранимая процедура для удаления книги
CREATE PROCEDURE usp_DeleteBook
    @BookID INT
AS
BEGIN
    DELETE FROM Books WHERE BookID = @BookID
END
GO