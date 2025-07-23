-- Insert sample enrollment data for student S001
-- This script creates enrollment records for testing the MyCourses page

-- First, make sure we have the student S001
IF NOT EXISTS (SELECT 1 FROM Student WHERE StudentID = 'S001')
BEGIN
    INSERT INTO Student (StudentID, FullName, Email, Phone, Address, DateOfBirth, Gender, EnrollmentDate)
    VALUES ('S001', 'Nguyen Van A', 'nguyenvana@email.com', '0123456789', 'Ha Noi', '1995-01-01', 'Male', GETDATE())
END

-- Insert sample course enrollments for S001
-- Student S001 will have 3 enrolled courses with different progress levels

-- Enrollment 1: C# Programming (75% complete)
IF NOT EXISTS (SELECT 1 FROM CourseEnrollment WHERE StudentID = 'S001' AND CourseID = 'CO001')
BEGIN
    INSERT INTO CourseEnrollment (StudentID, CourseID, EnrollmentDate, Status)
    VALUES ('S001', 'CO001', DATEADD(day, -30, GETDATE()), 'Active')
END

-- Enrollment 2: Web Development (45% complete)  
IF NOT EXISTS (SELECT 1 FROM CourseEnrollment WHERE StudentID = 'S001' AND CourseID = 'CO002')
BEGIN
    INSERT INTO CourseEnrollment (StudentID, CourseID, EnrollmentDate, Status)
    VALUES ('S001', 'CO002', DATEADD(day, -20, GETDATE()), 'Active')
END

-- Enrollment 3: Data Science (100% complete)
IF NOT EXISTS (SELECT 1 FROM CourseEnrollment WHERE StudentID = 'S001' AND CourseID = 'CO003')
BEGIN
    INSERT INTO CourseEnrollment (StudentID, CourseID, EnrollmentDate, Status)
    VALUES ('S001', 'CO003', DATEADD(day, -60, GETDATE()), 'Completed')
END

-- Insert sample payment records for these enrollments
-- Payment 1: For C# Programming
IF NOT EXISTS (SELECT 1 FROM Payment WHERE StudentID = 'S001' AND CourseID = 'CO001')
BEGIN
    INSERT INTO Payment (StudentID, CourseID, Amount, PaymentDate, PaymentMethod, Status)
    VALUES ('S001', 'CO001', 500000, DATEADD(day, -30, GETDATE()), 'CreditCard', 'Completed')
END

-- Payment 2: For Web Development
IF NOT EXISTS (SELECT 1 FROM Payment WHERE StudentID = 'S001' AND CourseID = 'CO002')
BEGIN
    INSERT INTO Payment (StudentID, CourseID, Amount, PaymentDate, PaymentMethod, Status)
    VALUES ('S001', 'CO002', 750000, DATEADD(day, -20, GETDATE()), 'BankTransfer', 'Completed')
END

-- Payment 3: For Data Science
IF NOT EXISTS (SELECT 1 FROM Payment WHERE StudentID = 'S001' AND CourseID = 'CO003')
BEGIN
    INSERT INTO Payment (StudentID, CourseID, Amount, PaymentDate, PaymentMethod, Status)
    VALUES ('S001', 'CO003', 900000, DATEADD(day, -60, GETDATE()), 'CreditCard', 'Completed')
END

-- Insert some sample lessons for progress calculation
-- Lessons for CO001 (C# Programming) - 20 lessons total
DECLARE @i INT = 1
WHILE @i <= 20
BEGIN
    IF NOT EXISTS (SELECT 1 FROM Lesson WHERE CourseID = 'CO001' AND LessonNumber = @i)
    BEGIN
        INSERT INTO Lesson (CourseID, Title, Content, LessonNumber, Duration, VideoUrl)
        VALUES ('CO001', 'C# Lesson ' + CAST(@i AS VARCHAR(2)), 'Content for lesson ' + CAST(@i AS VARCHAR(2)), @i, 30, 'https://example.com/lesson' + CAST(@i AS VARCHAR(2)))
    END
    SET @i = @i + 1
END

-- Lessons for CO002 (Web Development) - 20 lessons total
SET @i = 1
WHILE @i <= 20
BEGIN
    IF NOT EXISTS (SELECT 1 FROM Lesson WHERE CourseID = 'CO002' AND LessonNumber = @i)
    BEGIN
        INSERT INTO Lesson (CourseID, Title, Content, LessonNumber, Duration, VideoUrl)
        VALUES ('CO002', 'Web Dev Lesson ' + CAST(@i AS VARCHAR(2)), 'Content for lesson ' + CAST(@i AS VARCHAR(2)), @i, 45, 'https://example.com/webdev' + CAST(@i AS VARCHAR(2)))
    END
    SET @i = @i + 1
END

-- Lessons for CO003 (Data Science) - 25 lessons total
SET @i = 1
WHILE @i <= 25
BEGIN
    IF NOT EXISTS (SELECT 1 FROM Lesson WHERE CourseID = 'CO003' AND LessonNumber = @i)
    BEGIN
        INSERT INTO Lesson (CourseID, Title, Content, LessonNumber, Duration, VideoUrl)
        VALUES ('CO003', 'Data Science Lesson ' + CAST(@i AS VARCHAR(2)), 'Content for lesson ' + CAST(@i AS VARCHAR(2)), @i, 40, 'https://example.com/datascience' + CAST(@i AS VARCHAR(2)))
    END
    SET @i = @i + 1
END

PRINT 'Sample enrollment data inserted successfully!'
PRINT 'Student S001 now has 3 enrolled courses:'
PRINT '- CO001: C# Programming (20 lessons)'
PRINT '- CO002: Web Development (20 lessons)' 
PRINT '- CO003: Data Science (25 lessons)'
