-- Drop and recreate the database
IF DB_ID('Hikari') IS NOT NULL
    DROP DATABASE Hikari;
GO

CREATE DATABASE Hikari;
GO

USE Hikari;
GO

-- UserAccount table
CREATE TABLE UserAccount (
    userID VARCHAR(10) PRIMARY KEY CHECK (userID LIKE 'U[0-9][0-9][0-9]'),
    username VARCHAR(255) NOT NULL,
    fullName VARCHAR(255) NOT NULL,
    email VARCHAR(255) UNIQUE NOT NULL,
    password VARCHAR(255) NOT NULL,
    role VARCHAR(20) NOT NULL CHECK (role IN ('Student', 'Teacher', 'Admin', 'Coordinator')),
    registrationDate DATE DEFAULT CAST(GETDATE() AS DATE),
    profilePicture VARCHAR(255),
    phone VARCHAR(20),
    birthDate DATE
);
GO

-- Student table
CREATE TABLE Student (
    studentID VARCHAR(10) PRIMARY KEY CHECK (studentID LIKE 'S[0-9][0-9][0-9]'),
    userID VARCHAR(10) UNIQUE,
    enrollmentDate DATE DEFAULT CAST(GETDATE() AS DATE),
    FOREIGN KEY (userID) REFERENCES UserAccount(userID) ON DELETE CASCADE
);
GO

-- Teacher table
CREATE TABLE Teacher (
    teacherID VARCHAR(10) PRIMARY KEY CHECK (teacherID LIKE 'T[0-9][0-9][0-9]'),
    userID VARCHAR(10) UNIQUE NOT NULL,
    specialization VARCHAR(255),
    experienceYears INT CHECK (experienceYears >= 0),
    FOREIGN KEY (userID) REFERENCES UserAccount(userID) ON DELETE CASCADE
);
GO

-- Courses table
CREATE TABLE Courses (
    courseID VARCHAR(10) PRIMARY KEY CHECK (courseID LIKE 'CO[0-9][0-9][0-9]'),
    title VARCHAR(255) NOT NULL,
    description TEXT,
    fee DECIMAL(10, 2) DEFAULT 0.00,
    duration INT,
    imageUrl VARCHAR(500),
    startDate DATE,
    endDate DATE,
    isActive BIT DEFAULT 1,
    CHECK (endDate >= startDate)
);
GO

-- Course_Enrollments table
CREATE TABLE Course_Enrollments (
    enrollmentID VARCHAR(10) PRIMARY KEY CHECK (enrollmentID LIKE 'E[0-9][0-9][0-9]'),
    studentID VARCHAR(10) NOT NULL,
    courseID VARCHAR(10) NOT NULL,
    enrollmentDate DATE DEFAULT CAST(GETDATE() AS DATE),
    completionDate DATE,
    FOREIGN KEY (studentID) REFERENCES Student(studentID),
    FOREIGN KEY (courseID) REFERENCES Courses(courseID)
);
GO

-- Course_Reviews table
CREATE TABLE Course_Reviews (
    id INT IDENTITY(1,1) PRIMARY KEY,
    courseID VARCHAR(10) NOT NULL,
    userID VARCHAR(10) NOT NULL,
    rating INT CHECK (rating BETWEEN 1 AND 5),
    reviewText TEXT,
    reviewDate DATE DEFAULT CAST(GETDATE() AS DATE),
    FOREIGN KEY (courseID) REFERENCES Courses(courseID),
    FOREIGN KEY (userID) REFERENCES UserAccount(userID)
);
GO

-- Lesson table
CREATE TABLE Lesson (
    id INT IDENTITY(1,1) PRIMARY KEY,
    courseID VARCHAR(10) NOT NULL,
    title VARCHAR(255) NOT NULL,
    description TEXT,
    mediaUrl VARCHAR(500),
    duration INT,
    isCompleted BIT DEFAULT 0,
    isActive BIT DEFAULT 1,
    FOREIGN KEY (courseID) REFERENCES Courses(courseID) ON DELETE CASCADE
);
GO

-- Progress table
CREATE TABLE Progress (
    id INT IDENTITY(1,1) PRIMARY KEY,
    studentID VARCHAR(10) NOT NULL,
    enrollmentID VARCHAR(10) NOT NULL,
    lessonID INT NOT NULL,
    completionStatus VARCHAR(20) NOT NULL DEFAULT 'in progress' CHECK (completionStatus IN ('complete', 'in progress')),
    startDate DATE,
    endDate DATE,
    score DECIMAL(5, 2),
    feedback VARCHAR(500),
    FOREIGN KEY (studentID) REFERENCES Student(studentID),
    FOREIGN KEY (enrollmentID) REFERENCES Course_Enrollments(enrollmentID),
    FOREIGN KEY (lessonID) REFERENCES Lesson(id)
);
GO

-- Document table
CREATE TABLE Document (
    id INT IDENTITY(1,1) PRIMARY KEY,
    lessonID INT,
    title VARCHAR(255) NOT NULL,
    description TEXT,
    fileUrl VARCHAR(500),
    uploadDate DATETIME DEFAULT GETDATE(),
    uploadedBy VARCHAR(10),
    FOREIGN KEY (lessonID) REFERENCES Lesson(id),
    FOREIGN KEY (uploadedBy) REFERENCES Teacher(teacherID)
);
GO

-- Test table
CREATE TABLE Test (
    id INT IDENTITY(1,1) PRIMARY KEY,
    jlptLevel NVARCHAR(10) NOT NULL CHECK (jlptLevel IN ('N5', 'N4', 'N3', 'N2', 'N1')),
    title NVARCHAR(255) NOT NULL,
    description NVARCHAR(255),
    totalMarks DECIMAL(10,2) CHECK (totalMarks >= 0),
    totalQuestions INT CHECK (totalQuestions >= 0),
    isActive BIT DEFAULT 1
);
GO

-- Question table
CREATE TABLE Question (
    id INT IDENTITY(1,1) PRIMARY KEY,
    questionText NVARCHAR(255) NOT NULL,
    optionA NVARCHAR(255),
    optionB NVARCHAR(255),
    optionC NVARCHAR(255),
    optionD NVARCHAR(255),
    correctOption CHAR(1) CHECK (correctOption IN ('A', 'B', 'C', 'D')),
    mark DECIMAL(10,2) DEFAULT 1 CHECK (mark >= 0),
    entityType VARCHAR(20) NOT NULL CHECK (entityType IN ('assignment', 'exercise', 'test')),
    entityID INT NOT NULL
);
GO

-- Payment table
CREATE TABLE Payment (
    id INT IDENTITY(1,1) PRIMARY KEY,
    studentID VARCHAR(10) NOT NULL,
    enrollmentID VARCHAR(10) NOT NULL,
    amount DECIMAL(10, 2) NOT NULL,
    paymentMethod VARCHAR(50),
    paymentStatus VARCHAR(20) NOT NULL CHECK (paymentStatus IN ('Cancel', 'Pending', 'Complete')),
    paymentDate DATETIME DEFAULT GETDATE(),
    transactionID VARCHAR(100),
    FOREIGN KEY (studentID) REFERENCES Student(studentID),
    FOREIGN KEY (enrollmentID) REFERENCES Course_Enrollments(enrollmentID)
);
GO

-- Discount table
CREATE TABLE Discount (
    id INT IDENTITY(1,1) PRIMARY KEY,
    code VARCHAR(50) UNIQUE NOT NULL,
    courseID VARCHAR(10) NOT NULL,
    discountPercent INT CHECK (discountPercent BETWEEN 0 AND 100),
    startDate DATE,
    endDate DATE,
    isActive BIT DEFAULT 1,
    FOREIGN KEY (courseID) REFERENCES Courses(courseID),
    CHECK (endDate >= startDate)
);
GO

-- Exercise table
CREATE TABLE Exercise (
    id INT IDENTITY(1,1) PRIMARY KEY,
    lessonId INT NOT NULL UNIQUE,
    title NVARCHAR(255) NOT NULL,
    description NVARCHAR(MAX),
    duration INT, -- Duration in minutes
    passMark DECIMAL(5, 2) DEFAULT 50.00,
    FOREIGN KEY (lessonId) REFERENCES Lesson(id) ON DELETE CASCADE
);
GO
-- Insert data
-- 1. UserAccount
INSERT INTO UserAccount (userID, fullName, username, email, password, role, profilePicture, phone, birthDate) VALUES
    ('U001', 'Trần Đình Qúy', 'quy123', 'quy123@gmail.com', 'password123', 'Student', 'https://projectswp1.s3.ap-southeast-2.amazonaws.com/forum/avatars/U001/158e3847-a82d-46d3-a486-f04c2516c5a8.jpg', '0901234567', '1990-01-15'),
    ('U002', 'Trần Văn Tùng', 'tung123', 'tung123@gmail.com', 'password123', 'Teacher', 'https://projectswp1.s3.ap-southeast-2.amazonaws.com/forum/avatars/U002/88da79ef-e003-4c87-a9bf-25a9d9af778b.jpg', '0987654321', '1985-05-20'),
    ('U003', 'Lê Quốc Hùng', 'hung123', 'hung@gmail.com', 'password123', 'Student', 'https://projectswp1.s3.ap-southeast-2.amazonaws.com/forum/avatars/U005/bb3d2b76-26f5-4ab1-9cab-2edca1a72d7b.jpg', '0912345678', '1980-09-10'),
    ('U004', 'Phan Nguyễn Gia Huy', 'huy123', 'huy@gmail.com', 'password123', 'Coordinator', 'https://projectswp1.s3.ap-southeast-2.amazonaws.com/forum/avatars/U004/08907845-db49-48e8-81c5-17d2f2b31fb7.jpg', '0923456789', '1992-12-25'),
    ('U005', 'Vũ Lê Duy', 'duy123', 'duy@gmail.com', 'password123', 'Admin', 'https://projectswp1.s3.ap-southeast-2.amazonaws.com/forum/avatars/U005/17083591-9cf3-4015-bb7b-207e3aaf54e6.png', '0934567890', '1988-07-30'),
    ('U006', 'Lê Thu Trang', 'trang123', 'trang@gmail.com', 'password123', 'Student', '/assets/img/avatar.png', '0945678901', '1995-03-12'),
    ('U007', 'Bùi Thị Nhi', 'nhi123', 'nhi@gmail.com', 'password123', 'Teacher', 'https://projectswp1.s3.ap-southeast-2.amazonaws.com/forum/avatars/U007/8362e0ba-6c3f-4265-bbe9-7ce64be95c23.jpg', '0956789012', '1983-11-08'),
    ('U008', 'Nguyen Thanh Tam', 'tam123', 'tam@gmail.com', 'password123', 'Student', '/assets/img/avatar.png', '0967890123', '1998-06-22'),
    ('U009', 'Hoàng Văn Minh', 'minh123', 'minh@gmail.com', 'password123', 'Student', '/assets/img/avatar.png', '0978901234', '1997-02-14'),
    ('U010', 'Trần Thị Hoa', 'hoa123', 'hoa@gmail.com', 'password123', 'Teacher', '/assets/img/avatar.png', '0989012345', '1986-09-18'),
    ('U011', 'Võ Thị Thu', 'thu123', 'thu@gmail.com', 'password123', 'Student', '/assets/img/avatar.png', '0990123456', '1999-04-18'),
    ('U012', 'Đỗ Văn Khoa', 'khoa123', 'khoa@gmail.com', 'password123', 'Student', '/assets/img/avatar.png', '0901234568', '1996-08-07'),
    ('U013', 'Bùi Thị Linh', 'linh123', 'linh@gmail.com', 'password123', 'Student', '/assets/img/avatar.png', '0912345679', '1994-12-03'),
    ('U014', 'Ngô Văn Đức', 'duc123', 'duc@gmail.com', 'password123', 'Student', '/assets/img/avatar.png', '0923456780', '1993-10-15'),
    ('U015', 'Lý Thị An', 'an123', 'an@gmail.com', 'password123', 'Student', '/assets/img/avatar.png', '0934567891', '1991-05-28'),
    ('U016', 'Cao Thị Yến', 'yen123', 'yen@gmail.com', 'password123', 'Teacher', '/assets/img/avatar.png', '0945678902', '1982-03-25'),
    ('U017', 'Đinh Văn Sơn', 'son123', 'son@gmail.com', 'password123', 'Teacher', '/assets/img/avatar.png', '0956789013', '1984-07-12');

-- 2. Student (removed vote column)
INSERT INTO Student (studentID, userID, enrollmentDate) VALUES
    ('S001', 'U001', '2024-01-15'),
    ('S002', 'U003', '2024-02-20'),
    ('S003', 'U006', '2024-03-10'),
    ('S004', 'U008', '2024-04-05'),
    ('S005', 'U009', '2024-05-12'),
    ('S006', 'U011', '2024-06-18'),
    ('S007', 'U012', '2024-07-22'),
    ('S008', 'U013', '2024-08-08'),
    ('S009', 'U014', '2024-09-14'),
    ('S010', 'U015', '2024-10-25');

-- 3. Teacher
INSERT INTO Teacher (teacherID, userID, specialization, experienceYears) VALUES
    ('T001', 'U002', 'Ngữ pháp tiếng Nhật', 8),
    ('T002', 'U007', 'Kanji và Từ vựng', 5),
    ('T003', 'U010', 'Hội thoại tiếng Nhật', 12),
    ('T004', 'U016', 'JLPT N1-N2', 10),
    ('T005', 'U017', 'Văn hóa Nhật Bản', 6);

-- 4. Courses
INSERT INTO Courses (courseID, title, description, fee, duration, startDate, endDate, isActive) VALUES
    ('CO001', 'Tiếng Nhật Cơ Bản N5', 'Khóa học tiếng Nhật cho người mới bắt đầu', 2000000.00, 120, '2024-01-15', '2024-05-15', 0),
    ('CO002', 'Tiếng Nhật Trung Cấp N4', 'Khóa học tiếng Nhật trình độ trung cấp', 2500000.00, 150, '2024-02-01', '2024-07-01', 0),
    ('CO003', 'Tiếng Nhật Cao Cấp N3', 'Khóa học tiếng Nhật trình độ cao cấp', 3000000.00, 180, '2024-03-01', '2024-09-01', 0),
    ('CO004', 'JLPT N2 Luyện Thi', 'Khóa học luyện thi JLPT N2', 3500000.00, 200, '2024-04-01', '2024-11-01', 0),
    ('CO005', 'JLPT N1 Chuyên Sâu', 'Khóa học chuyên sâu cho JLPT N1', 4000000.00, 240, '2024-05-01', '2024-12-01', 0),
    ('CO006', 'Kanji Mastery', 'Khóa học chuyên về Kanji', 1800000.00, 100, '2025-05-01', '2025-08-01', 1),
    ('CO007', 'Hội Thoại Thực Tế', 'Khóa học hội thoại tiếng Nhật thực tế', 2200000.00, 80, '2025-06-01', '2025-08-15', 1),
    ('CO008', 'Văn Hóa Nhật Bản', 'Tìm hiểu văn hóa và xã hội Nhật Bản', 1500000.00, 60, '2025-07-01', '2025-09-01', 1),
    ('CO009', 'Tiếng Nhật Thương Mại', 'Tiếng Nhật cho môi trường công việc', 3200000.00, 160, '2025-06-15', '2025-11-15', 1),
    ('CO010', 'Luyện Nghe N3-N2', 'Khóa học chuyên luyện kỹ năng nghe', 1900000.00, 90, '2025-06-20', '2025-09-20', 1),
    ('CO011', 'Tiếng Nhật Cơ Bản N5 - Khóa 2', 'Khóa học tiếng Nhật cho người mới bắt đầu - Khóa 2', 2000000.00, 120, '2024-06-23', '2024-08-15', 0),
    ('CO012', 'Văn Hóa Nhật Bản - Khóa Mở Rộng', 'Tìm hiểu văn hóa và xã hội Nhật Bản - Khóa mở rộng', 1500000.00, 60, '2025-07-01', '2025-09-01', 1);

-- 5. Course_Enrollments
INSERT INTO Course_Enrollments (enrollmentID, studentID, courseID, enrollmentDate, completionDate) VALUES
    ('E001', 'S001', 'CO001', '2024-01-15', NULL),
    ('E002', 'S002', 'CO002', '2024-02-20', NULL),
    ('E003', 'S003', 'CO001', '2024-03-10', NULL),
    ('E004', 'S004', 'CO003', '2024-04-05', NULL),
    ('E005', 'S005', 'CO002', '2024-05-12', NULL),
    ('E006', 'S006', 'CO004', '2024-06-18', NULL),
    ('E007', 'S007', 'CO003', '2024-07-22', NULL),
    ('E008', 'S008', 'CO005', '2024-08-08', NULL),
    ('E009', 'S009', 'CO001', '2024-09-14', NULL),
    ('E010', 'S010', 'CO006', '2024-10-25', NULL);

-- 6. Course_Reviews
INSERT INTO Course_Reviews (courseID, userID, rating, reviewText, reviewDate) VALUES
    ('CO001', 'U001', 5, 'Khóa học rất hay, giảng viên nhiệt tình', '2024-05-25'),
    ('CO002', 'U003', 4, 'Nội dung phong phú, cần thêm bài tập', '2024-06-10'),
    ('CO001', 'U006', 5, 'Phù hợp cho người mới bắt đầu', '2024-05-28'),
    ('CO003', 'U008', 4, 'Khóa học chất lượng cao', '2024-07-15'),
    ('CO002', 'U009', 3, 'Tốc độ hơi nhanh với tôi', '2024-06-20'),
    ('CO004', 'U001', 5, 'Chuẩn bị thi JLPT rất tốt', '2024-08-01'),
    ('CO003', 'U003', 4, 'Giảng viên giải thích rõ ràng', '2024-07-20'),
    ('CO005', 'U006', 5, 'Khóa học chuyên sâu và hữu ích', '2024-09-01'),
    ('CO001', 'U008', 4, 'Bài học được sắp xếp logic', '2024-05-30'),
    ('CO006', 'U009', 5, 'Học Kanji hiệu quả', '2024-09-15');

-- 7. Payment
INSERT INTO Payment (studentID, enrollmentID, amount, paymentMethod, paymentStatus, paymentDate, transactionID) VALUES
    ('S001', 'E001', 2000000.00, 'Bank Transfer', 'Complete', '2025-01-20 10:00:00', 'TXN001'),
    ('S002', 'E002', 2500000.00, 'Credit Card', 'Cancel', '2025-02-05 14:30:00', 'TXN002'),
    ('S003', 'E003', 2000000.00, 'Mobile Payment', 'Complete', '2025-03-10 09:15:00', 'TXN003'),
    ('S004', 'E004', 3000000.00, 'Bank Transfer', 'Cancel', '2025-04-05 16:45:00', 'TXN004'),
    ('S005', 'E005', 2500000.00, 'Credit Card', 'Pending', '2025-05-12 11:00:00', 'TXN005'),
    ('S006', 'E006', 3500000.00, 'Mobile Payment', 'Complete', '2025-06-18 13:20:00', 'TXN006'),
    ('S007', 'E007', 3000000.00, 'Bank Transfer', 'Pending', '2025-07-02 15:30:00', 'TXN007'),
    ('S008', 'E008', 4000000.00, 'Credit Card', 'Complete', '2025-06-08 09:45:00', 'TXN008'),
    ('S009', 'E009', 2000000.00, 'Mobile Payment', 'Cancel', '2025-07-14 10:10:00', 'TXN009'),
    ('S010', 'E010', 1800000.00, 'Bank Transfer', 'Pending', '2025-06-25 14:00:00', 'TXN010');

-- 8. Discount
INSERT INTO Discount (code, courseID, discountPercent, startDate, endDate, isActive) VALUES
    ('NEWBIE2024', 'CO001', 20, '2024-01-01', '2024-03-31', 1),
    ('SUMMER2024', 'CO002', 15, '2024-06-01', '2024-08-31', 1),
    ('JLPTN3SALE', 'CO003', 25, '2024-04-01', '2024-06-30', 1),
    ('EARLYBIRD', 'CO004', 30, '2024-03-01', '2024-04-30', 1),
    ('PREMIUM2024', 'CO005', 10, '2024-05-01', '2024-12-31', 1),
    ('KANJI50', 'CO006', 40, '2024-06-01', '2024-07-31', 1),
    ('SPEAKING', 'CO007', 20, '2024-07-01', '2024-08-31', 1);
GO

-- 9. Test
INSERT INTO Test (jlptLevel, title, description, totalMarks, totalQuestions, isActive) VALUES
(N'N5', N'Bài kiểm tra luyện thi JLPT N5', N'Bài kiểm tra cấp độ sơ cấp dành cho việc chuẩn bị JLPT N5.', 100, 50, 1),
(N'N4', N'Bài thi thử JLPT N4', N'Bài kiểm tra trung cấp bao quát chương trình JLPT N4.', 100, 75, 1),
(N'N3', N'Đánh giá JLPT N3', N'Bài kiểm tra toàn diện dành cho thí sinh JLPT N3.', 100, 90, 0),
(N'N2', N'Bài kiểm tra nâng cao JLPT N2', N'Bài kiểm tra thử thách dành cho người học JLPT N2 nâng cao.', 200.00, 100, 1),
(N'N1', N'Bài kiểm tra trình độ JLPT N1', N'Bài kiểm tra cấp độ chuyên gia cho chứng chỉ JLPT N1.', 100, 100, 1);

INSERT INTO Question (questionText, optionA, optionB, optionC, optionD, correctOption, mark, entityType, entityID) VALUES
(N'N5 Q1: Chọn từ đúng: 「これは___です。」', N'本', N'車', N'家', N'犬', 'A', 1.00, N'test', 1),
(N'N5 Q2: Chọn câu đúng: 「私は日本人です。」', N'私は日本人です。', N'日本人です私。', N'私は日本人。', N'日本人です。', 'A', 1.00, N'test', 1),
(N'N5 Q3: Chữ Hán nào là "người"?', N'人', N'日', N'月', N'火', 'A', 1.00, N'test', 1),
(N'N5 Q4: Dịch: "Tên tôi là Tanaka."', N'私の名前は田中です。', N'私の名前田中です。', N'田中名前です。', N'私は田中。', 'A', 1.00, N'test', 1),
(N'N5 Q5: Chọn từ trái nghĩa với "高い" (cao):', N'低い', N'広い', N'狭い', N'速い', 'A', 1.00, N'test', 1);

INSERT INTO Question (questionText, optionA, optionB, optionC, optionD, correctOption, mark, entityType, entityID) VALUES
(N'N4 Q1: Chọn trợ từ phù hợp: 「雨が降って___、行きました。」', N'も', N'から', N'のに', N'ので', 'C', 1.00, N'test', 2),
(N'N4 Q2: Chọn câu đúng về ý nghĩa 「～てしまう」:', N'Làm xong hết', N'Đáng tiếc đã làm mất', N'Cả A và B', N'Chỉ A', 'C', 1.00, N'test', 2),
(N'N4 Q3: Chữ Hán nào là "ga/nhà ga"?', N'駅', N'店', N'家', N'道', 'A', 1.00, N'test', 2),
(N'N4 Q4: Dịch: "Tôi có thể nói tiếng Nhật một chút."', N'日本語が少し話せます。', N'日本語を少し話します。', N'日本語が少し話します。', N'日本語は少し話します。', 'A', 1.00, N'test', 2),
(N'N4 Q5: Chọn từ đồng nghĩa với "たくさん":', N'たくさん', N'たくさんあります', N'たくさんいます', N'たくさんではない', 'A', 1.00, N'test', 2);

INSERT INTO Question (questionText, optionA, optionB, optionC, optionD, correctOption, mark, entityType, entityID) VALUES
(N'N3 Q1: Ý nghĩa của cấu trúc 「～わけがない」?', N'Không thể nào (không có lý do)', N'Không có ý định', N'Không cần thiết', N'Không phải là không', 'A', 1.00, N'test', 3),
(N'N3 Q2: Chọn từ phù hợp: 「彼は日本語を勉強した___、話せるようになった。」', N'おかげで', N'せいで', N'わりに', N'にもかかわらず', 'A', 1.00, N'test', 3),
(N'N3 Q3: Chữ Hán nào là "kinh nghiệm"?', N'経験', N'経済', N'経営', N'計算', 'A', 1.00, N'test', 3),
(N'N3 Q4: Dịch: "Anh ấy có vẻ không khỏe."', N'彼は元気がないようだ。', N'彼は元気がありません。', N'彼は元気じゃない。', N'彼は元気ではない。', 'A', 1.00, N'test', 3),
(N'N3 Q5: Chọn câu đúng khi muốn nói "có thể nhìn thấy" một cách tự nhiên:', N'見える', N'見られる', N'見る', N'見ます', 'A', 1.00, N'test', 3);

INSERT INTO Question (questionText, optionA, optionB, optionC, optionD, correctOption, mark, entityType, entityID) VALUES
(N'N2 Q1: Ý nghĩa của cấu trúc 「～どころではない」?', N'Không phải lúc/không thể làm gì đó', N'Không phải nơi nào đó', N'Không phải là không làm', N'Không có chỗ nào', 'A', 1.00, N'test', 4),
(N'N2 Q2: Chọn từ phù hợp: 「彼は仕事___、食事もとらない。」', N'ばかりか', N'ばかりに', N'ばかりでなく', N'ばかりだ', 'C', 1.00, N'test', 4),
(N'N2 Q3: Chữ Hán nào là "công việc"?', N'仕事', N'使用', N'仕方', N'資本', 'A', 1.00, N'test', 4),
(N'N2 Q4: Dịch: "Dù trời mưa nhưng anh ấy vẫn đi ra ngoài."', N'雨なのに彼は出かけた。', N'雨だから彼は出かけた。', N'雨が降って彼は出かけた。', N'雨が降ると彼は出かけた。', 'A', 1.00, N'test', 4),
(N'N2 Q5: Chọn cách diễn đạt đúng khi muốn thể hiện một điều hiển nhiên:', N'～に決まっている', N'～はずだ', N'～ことだ', N'～だろう', 'A', 1.00, N'test', 4);
-- Bài học cho khóa 'CO001' - Lập trình C# Cơ bản
-- Khóa học: CO001 - Tiếng Nhật Sơ Cấp 1 (Minna no Nihongo I)
INSERT INTO Lesson (courseID, title, description, mediaUrl, duration, isCompleted, isActive) VALUES
('CO001', 'Bài 1: Giới thiệu Tiếng Nhật & Chào hỏi cơ bản', 'Làm quen với tiếng Nhật, cách chào hỏi thông dụng và giới thiệu bản thân.', 'https://www.youtube.com/watch?v=6p9Il_j0zjc', 60, 0, 1),
('CO001', 'Bài 2: Bảng chữ cái Hiragana (Phần 1)', 'Học cách đọc và viết các hàng a, ka, sa, ta, na trong Hiragana.', 'https://www.youtube.com/watch?v=s6DKRgtVLGA', 55, 0, 1),
('CO001', 'Bài 3: Bảng chữ cái Hiragana (Phần 2)', 'Học cách đọc và viết các hàng ha, ma, ya, ra, wa, n trong Hiragana.', 'https://www.youtube.com/watch?v=rGrBHiuPlT0', 60, 0, 1),
('CO001', 'Bài 4: Katakana và Dấu trường âm, âm ngắt', 'Làm quen với Katakana, quy tắc phát âm trường âm và âm ngắt.', 'https://www.youtube.com/watch?v=9M0Gb0Q_F8k', 50, 0, 1);

-- Khóa học: CO002 - Tiếng Nhật Sơ Cấp 2 (Minna no Nihongo II)
INSERT INTO Lesson (courseID, title, description, mediaUrl, duration, isCompleted, isActive) VALUES
('CO002', 'Bài 1: Thể Te và ứng dụng', 'Học cách chia động từ sang thể Te và các cách dùng cơ bản.', 'https://www.youtube.com/watch?v=6p9Il_j0zjc', 70, 0, 1),
('CO002', 'Bài 2: Thể Ta và kinh nghiệm', 'Học cách chia động từ sang thể Ta và diễn tả kinh nghiệm đã từng làm.', 'https://www.youtube.com/watch?v=s6DKRgtVLGA', 65, 0, 1),
('CO002', 'Bài 3: Dạng ngắn (Thể thông thường)', 'Tìm hiểu về dạng ngắn của động từ, tính từ và danh từ.', 'https://www.youtube.com/watch?v=rGrBHiuPlT0', 80, 0, 1);

-- Khóa học: CO003 - Ngữ Pháp Tiếng Nhật N5
INSERT INTO Lesson (courseID, title, description, mediaUrl, duration, isCompleted, isActive) VALUES
('CO003', 'Bài 1: Cấu trúc câu cơ bản (S + O + V)', 'Nắm vững cấu trúc câu đơn giản và các trợ từ cơ bản.', 'https://www.youtube.com/watch?v=6p9Il_j0zjc', 50, 0, 1),
('CO003', 'Bài 2: Các trợ từ cơ bản (ni, de, he, to, kara, made)', 'Tìm hiểu ý nghĩa và cách dùng các trợ từ phổ biến.', 'https://www.youtube.com/watch?v=s6DKRgtVLGA', 55, 0, 1),
('CO003', 'Bài 3: Biểu hiện sự tồn tại (imasu, arimasu)', 'Học cách diễn tả sự tồn tại của người và vật.', 'https://www.youtube.com/watch?v=rGrBHiuPlT0', 60, 0, 1);

-- Khóa học: CO004 - Kanji Cơ Bản (N5-N4)
INSERT INTO Lesson (courseID, title, description, mediaUrl, duration, isCompleted, isActive) VALUES
('CO004', 'Bài 1: Giới thiệu Kanji và bộ thủ cơ bản', 'Làm quen với Kanji, tầm quan trọng của bộ thủ và cách tra cứu.', 'https://www.youtube.com/watch?v=6p9Il_j0zjc', 60, 0, 1),
('CO004', 'Bài 2: Kanji về số đếm, thời gian, phương hướng', 'Học các chữ Hán liên quan đến số, ngày, tháng, năm và các hướng.', 'https://www.youtube.com/watch?v=s6DKRgtVLGA', 70, 0, 1),
('CO004', 'Bài 3: Kanji về con người, gia đình, nghề nghiệp', 'Học các chữ Hán miêu tả con người và các mối quan hệ xã hội.', 'https://www.youtube.com/watch?v=rGrBHiuPlT0', 65, 0, 1);

-- Khóa học: CO005 - Luyện Nghe - Nói Tiếng Nhật Sơ Cấp
INSERT INTO Lesson (courseID, title, description, mediaUrl, duration, isCompleted, isActive) VALUES
('CO005', 'Bài 1: Hội thoại hàng ngày - Giới thiệu bản thân', 'Thực hành nghe và nói các mẫu câu giới thiệu thông tin cá nhân.', 'https://www.youtube.com/watch?v=6p9Il_j0zjc', 80, 0, 1),
('CO005', 'Bài 2: Nghe hiểu thông báo và hướng dẫn đơn giản', 'Luyện kỹ năng nghe các thông tin cơ bản trong đời sống hàng ngày.', 'https://www.youtube.com/watch?v=s6DKRgtVLGA', 75, 0, 1),
('CO005', 'Bài 3: Đặt câu hỏi và trả lời thông thường', 'Thực hành giao tiếp qua các câu hỏi và câu trả lời thường gặp.', 'https://www.youtube.com/watch?v=rGrBHiuPlT0', 70, 0, 1);

-- Khóa học: CO006 - Tiếng Nhật cho người mới bắt đầu (Kanji cơ bản) (Giữ nguyên như bạn đã cung cấp)
INSERT INTO Lesson (courseID, title, description, mediaUrl, duration, isCompleted, isActive) VALUES
('CO006', 'Bài 1: Giới thiệu Kanji và bộ thủ', 'Làm quen với Kanji và các bộ thủ cơ bản.', 'https://www.youtube.com/watch?v=6p9Il_j0zjc', 60, 0, 1),
('CO006', 'Bài 2: Kanji về thiên nhiên', 'Học các Kanji liên quan đến núi, sông, cây, hoa.', 'https://www.youtube.com/watch?v=s6DKRgtVLGA', 55, 0, 1),
('CO006', 'Bài 3: Kanji về con người và gia đình', 'Học các Kanji về người, cha, mẹ, anh, chị, em.', 'https://www.youtube.com/watch?v=rGrBHiuPlT0', 50, 0, 1);

-- Khóa học: CO007 - Tiếng Nhật cho Du lịch & Giao tiếp cơ bản
INSERT INTO Lesson (courseID, title, description, mediaUrl, duration, isCompleted, isActive) VALUES
('CO007', 'Bài 1: Đặt phòng và hỏi đường', 'Các mẫu câu cần thiết khi đi du lịch, hỏi đường.', 'https://www.youtube.com/watch?v=6p9Il_j0zjc', 90, 0, 1),
('CO007', 'Bài 2: Giao tiếp tại nhà hàng và mua sắm', 'Học cách gọi món, thanh toán, hỏi giá khi mua sắm.', 'https://www.youtube.com/watch?v=s6DKRgtVLGA', 85, 0, 1),
('CO007', 'Bài 3: Xử lý tình huống khẩn cấp', 'Các cụm từ cần thiết khi gặp sự cố, bệnh tật.', 'https://www.youtube.com/watch?v=rGrBHiuPlT0', 70, 0, 1);

-- Khóa học: CO008 - Tiếng Nhật Thương Mại & Văn hóa Công sở
INSERT INTO Lesson (courseID, title, description, mediaUrl, duration, isCompleted, isActive) VALUES
('CO008', 'Bài 1: Giao tiếp email và điện thoại trong kinh doanh', 'Học cách viết email, trả lời điện thoại một cách lịch sự và chuyên nghiệp.', 'https://www.youtube.com/watch?v=6p9Il_j0zjc', 60, 0, 1),
('CO008', 'Bài 2: Kính ngữ (Keigo) cơ bản', 'Tìm hiểu các cấp độ kính ngữ và cách sử dụng trong công việc.', 'https://www.youtube.com/watch?v=s6DKRgtVLGA', 65, 0, 1),
('CO008', 'Bài 3: Thảo luận và đàm phán trong môi trường công sở', 'Thực hành các mẫu câu dùng trong các cuộc họp, đàm phán.', 'https://www.youtube.com/watch?v=rGrBHiuPlT0', 70, 0, 1);

-- Khóa học: CO009 - Đọc Hiểu Tiếng Nhật Trung Cấp
INSERT INTO Lesson (courseID, title, description, mediaUrl, duration, isCompleted, isActive) VALUES
('CO009', 'Bài 1: Chiến lược đọc hiểu báo chí và tin tức', 'Kỹ thuật đọc nhanh và nắm bắt ý chính từ các bài báo.', 'https://www.youtube.com/watch?v=6p9Il_j0zjc', 50, 0, 1),
('CO009', 'Bài 2: Đọc hiểu văn bản học thuật và chuyên ngành', 'Phân tích cấu trúc câu phức tạp và từ vựng chuyên ngành.', 'https://www.youtube.com/watch?v=s6DKRgtVLGA', 55, 0, 1),
('CO009', 'Bài 3: Luyện tập với các dạng bài JLPT N3/N2 về đọc hiểu', 'Thực hành giải các đề thi đọc hiểu cấp độ trung cấp.', 'https://www.youtube.com/watch?v=rGrBHiuPlT0', 60, 0, 1);

-- Khóa học: CO010 - Luyện Thi JLPT N4
INSERT INTO Lesson (courseID, title, description, mediaUrl, duration, isCompleted, isActive) VALUES
('CO010', 'Bài 1: Ngữ pháp N4 trọng tâm', 'Ôn tập và luyện tập các mẫu ngữ pháp quan trọng của N4.', 'https://www.youtube.com/watch?v=6p9Il_j0zjc', 75, 0, 1),
('CO010', 'Bài 2: Từ vựng N4 và phương pháp ghi nhớ', 'Mở rộng vốn từ vựng và các kỹ thuật học từ hiệu quả.', 'https://www.youtube.com/watch?v=s6DKRgtVLGA', 70, 0, 1),
('CO010', 'Bài 3: Luyện nghe và đọc hiểu N4 theo đề thi', 'Thực hành các dạng bài nghe và đọc hiểu mô phỏng đề thi thật.', 'https://www.youtube.com/watch?v=rGrBHiuPlT0', 80, 0, 1);

-- Khóa học: CO011 - Luyện Thi JLPT N3
INSERT INTO Lesson (courseID, title, description, mediaUrl, duration, isCompleted, isActive) VALUES
('CO011', 'Bài 1: Ngữ pháp N3 nâng cao và cách phân biệt', 'Nghiên cứu các mẫu ngữ pháp phức tạp và phân biệt các mẫu gần giống nhau.', 'https://www.youtube.com/watch?v=6p9Il_j0zjc', 65, 0, 1),
('CO011', 'Bài 2: Kanji và Từ vựng N3 chuyên sâu', 'Học các chữ Hán và từ vựng thường gặp ở cấp độ N3.', 'https://www.youtube.com/watch?v=s6DKRgtVLGA', 70, 0, 1),
('CO011', 'Bài 3: Chiến lược làm bài nghe và đọc hiểu N3', 'Các mẹo và chiến thuật để đạt điểm cao trong phần nghe và đọc hiểu.', 'https://www.youtube.com/watch?v=rGrBHiuPlT0', 75, 0, 1);

-- Khóa học: CO012 - Tiếng Nhật Nâng Cao & Văn học
INSERT INTO Lesson (courseID, title, description, mediaUrl, duration, isCompleted, isActive) VALUES
('CO012', 'Bài 1: Ngữ pháp và Biểu hiện N2/N1', 'Đi sâu vào các cấu trúc ngữ pháp phức tạp của N2/N1.', 'https://www.youtube.com/watch?v=6p9Il_j0zjc', 60, 0, 1),
('CO012', 'Bài 2: Đọc hiểu văn học Nhật Bản', 'Phân tích các đoạn trích từ tiểu thuyết, truyện ngắn Nhật Bản.', 'https://www.youtube.com/watch?v=s6DKRgtVLGA', 55, 0, 1),
('CO012', 'Bài 3: Thảo luận và thuyết trình bằng tiếng Nhật', 'Luyện kỹ năng giao tiếp nâng cao, thảo luận các chủ đề phức tạp.', 'https://www.youtube.com/watch?v=rGrBHiuPlT0', 50, 0, 1);

--CREATE TABLE [dbo].[Exercise](
--	[Id] [int] IDENTITY(1,1) NOT NULL,
--	[LessonId] [int] NOT NULL,
--	[Title] [nvarchar](255) NOT NULL,
--	[Description] [nvarchar](max) NULL,
--	[PassMark] [int] NULL,
--    CONSTRAINT [PK_Exercise] PRIMARY KEY CLUSTERED 
--    (
--        [Id] ASC
--    ),
--    CONSTRAINT [FK_Exercise_Lesson] FOREIGN KEY([LessonId])
--    REFERENCES [dbo].[Lesson] ([Id])
--    ON DELETE CASCADE
--);

--  DBCC CHECKIDENT ('[dbo].[Exercise]', RESEED, 0);
--  delete from [dbo].[Exercise]

-- Insert bài tập cho tất cả các bài học trong bảng Lesson

-- Khóa học: CO001 - Tiếng Nhật Sơ Cấp 1 (Minna no Nihongo I)
INSERT INTO [dbo].[Exercise] ([LessonId], [Title], [Description], [PassMark]) VALUES
(1, N'Bài tập chào hỏi và giới thiệu', N'Luyện tập các mẫu câu chào hỏi và giới thiệu bản thân bằng tiếng Nhật.', 80),
(2, N'Bài tập viết Hiragana (Phần 1)', N'Viết và nhận diện các chữ cái Hiragana thuộc hàng a, ka, sa, ta, na.', 75),
(3, N'Bài tập viết Hiragana (Phần 2)', N'Viết và nhận diện các chữ cái Hiragana thuộc hàng ha, ma, ya, ra, wa, n.', 75),
(4, N'Bài tập Katakana và phát âm', N'Luyện viết Katakana và nhận biết các âm trường âm, âm ngắt.', 70);

-- Khóa học: CO002 - Tiếng Nhật Sơ Cấp 2 (Minna no Nihongo II)
INSERT INTO [dbo].[Exercise] ([LessonId], [Title], [Description], [PassMark]) VALUES
(5, N'Bài tập chia động từ thể Te', N'Luyện tập chia động từ sang thể Te và áp dụng vào câu.', 80),
(6, N'Bài tập thể Ta và diễn tả kinh nghiệm', N'Hoàn thành các câu sử dụng thể Ta để diễn tả trải nghiệm.', 75),
(7, N'Bài tập dạng ngắn (thể thông thường)', N'Luyện tập chuyển đổi động từ, tính từ sang dạng ngắn.', 70);

-- Khóa học: CO003 - Ngữ Pháp Tiếng Nhật N5
INSERT INTO [dbo].[Exercise] ([LessonId], [Title], [Description], [PassMark]) VALUES
(8, N'Bài tập cấu trúc câu cơ bản', N'Hoàn thành câu sử dụng cấu trúc S + O + V và các trợ từ.', 75),
(9, N'Bài tập sử dụng trợ từ', N'Luyện tập chọn trợ từ phù hợp (ni, de, he, to, kara, made).', 70),
(10, N'Bài tập biểu hiện sự tồn tại', N'Hoàn thành câu sử dụng imasu và arimasu đúng ngữ cảnh.', 80);

-- Khóa học: CO004 - Kanji Cơ Bản (N5-N4)
INSERT INTO [dbo].[Exercise] ([LessonId], [Title], [Description], [PassMark]) VALUES
(11, N'Bài tập nhận diện bộ thủ Kanji', N'Xác định bộ thủ và ý nghĩa của các Kanji cơ bản.', 75),
(12, N'Bài tập Kanji số đếm và thời gian', N'Luyện viết và đọc các Kanji liên quan đến số, ngày, tháng.', 80),
(13, N'Bài tập Kanji về con người', N'Hoàn thành câu sử dụng Kanji về gia đình và nghề nghiệp.', 70);

-- Khóa học: CO005 - Luyện Nghe - Nói Tiếng Nhật Sơ Cấp
INSERT INTO [dbo].[Exercise] ([LessonId], [Title], [Description], [PassMark]) VALUES
(14, N'Bài tập hội thoại giới thiệu', N'Nghe và trả lời các câu hỏi về giới thiệu bản thân.', 75),
(15, N'Bài tập nghe thông báo', N'Luyện nghe và chọn đáp án đúng từ các thông báo đơn giản.', 70),
(16, N'Bài tập đặt câu hỏi', N'Thực hành đặt câu hỏi và trả lời bằng tiếng Nhật.', 80);

-- Khóa học: CO006 - Tiếng Nhật cho người mới bắt đầu (Kanji cơ bản)
INSERT INTO [dbo].[Exercise] ([LessonId], [Title], [Description], [PassMark]) VALUES
(17, N'Bài tập nhận diện Kanji cơ bản', N'Luyện tập nhận biết Kanji và các bộ thủ cơ bản.', 75),
(18, N'Bài tập Kanji về thiên nhiên', N'Hoàn thành câu sử dụng Kanji về núi, sông, cây, hoa.', 70),
(19, N'Bài tập Kanji về con người', N'Luyện viết và sử dụng Kanji về người, gia đình.', 80);

-- Khóa học: CO007 - Tiếng Nhật cho Du lịch & Giao tiếp cơ bản
INSERT INTO [dbo].[Exercise] ([LessonId], [Title], [Description], [PassMark]) VALUES
(20, N'Bài tập đặt phòng và hỏi đường', N'Thực hành các mẫu câu hỏi đường và đặt phòng khách sạn.', 75),
(21, N'Bài tập giao tiếp tại nhà hàng', N'Luyện tập gọi món ăn và hỏi giá tại nhà hàng.', 70),
(22, N'Bài tập xử lý tình huống khẩn cấp', N'Hoàn thành câu trong các tình huống khẩn cấp bằng tiếng Nhật.', 80);

-- Khóa học: CO008 - Tiếng Nhật Thương Mại & Văn hóa Công sở
INSERT INTO [dbo].[Exercise] ([LessonId], [Title], [Description], [PassMark]) VALUES
(23, N'Bài tập viết email kinh doanh', N'Viết email công việc bằng tiếng Nhật theo mẫu.', 80),
(24, N'Bài tập kính ngữ cơ bản', N'Luyện tập sử dụng kính ngữ trong các tình huống công sở.', 75),
(25, N'Bài tập thảo luận công việc', N'Thực hành các mẫu câu dùng trong họp và đàm phán.', 70);

-- Khóa học: CO009 - Đọc Hiểu Tiếng Nhật Trung Cấp
INSERT INTO [dbo].[Exercise] ([LessonId], [Title], [Description], [PassMark]) VALUES
(26, N'Bài tập đọc hiểu báo chí', N'Đọc và trả lời câu hỏi từ các đoạn báo ngắn.', 75),
(27, N'Bài tập phân tích văn bản học thuật', N'Phân tích cấu trúc câu và từ vựng trong văn bản chuyên ngành.', 80),
(28, N'Bài tập luyện thi đọc hiểu JLPT N3', N'Giải các đề thi đọc hiểu cấp độ trung cấp.', 70);

-- Khóa học: CO010 - Luyện Thi JLPT N4
INSERT INTO [dbo].[Exercise] ([LessonId], [Title], [Description], [PassMark]) VALUES
(29, N'Bài tập ngữ pháp N4', N'Ôn tập các mẫu ngữ pháp trọng tâm của JLPT N4.', 80),
(30, N'Bài tập từ vựng N4', N'Luyện tập từ vựng và chọn đáp án đúng theo đề thi N4.', 75),
(31, N'Bài tập nghe và đọc hiểu N4', N'Thực hành các dạng bài nghe và đọc hiểu của JLPT N4.', 70);

-- Khóa học: CO011 - Luyện Thi JLPT N3
INSERT INTO [dbo].[Exercise] ([LessonId], [Title], [Description], [PassMark]) VALUES
(32, N'Bài tập ngữ pháp N3 nâng cao', N'Luyện tập phân biệt các mẫu ngữ pháp phức tạp của N3.', 80),
(33, N'Bài tập Kanji và từ vựng N3', N'Ôn tập Kanji và từ vựng thường gặp trong JLPT N3.', 75),
(34, N'Bài tập nghe và đọc hiểu N3', N'Thực hành các dạng bài nghe và đọc hiểu của JLPT N3.', 70);

-- Khóa học: CO012 - Tiếng Nhật Nâng Cao & Văn học
INSERT INTO [dbo].[Exercise] ([LessonId], [Title], [Description], [PassMark]) VALUES
(35, N'Bài tập ngữ pháp N2/N1', N'Luyện tập các cấu trúc ngữ pháp phức tạp của JLPT N2/N1.', 80),
(36, N'Bài tập đọc hiểu văn học', N'Phân tích và trả lời câu hỏi từ các đoạn trích văn học.', 75),
(37, N'Bài tập thảo luận bằng tiếng Nhật', N'Thực hành thảo luận và thuyết trình về các chủ đề phức tạp.', 70);

-- Insert câu hỏi cho các bài tập trong bảng Exercise
-- Mỗi bài tập có 5 câu hỏi trắc nghiệm, thuộc entityType = 'exercise'

-- Bài tập 1 (LessonId = 1): Bài tập chào hỏi và giới thiệu
INSERT INTO Question (questionText, optionA, optionB, optionC, optionD, correctOption, mark, entityType, entityID) VALUES
(N'Chọn câu chào hỏi đúng vào buổi sáng:', N'おはよう', N'こんにちは', N'こんばんは', N'おやすみ', 'A', 1.00, 'exercise', 1),
(N'Dịch: "Tên tôi là Yamada."', N'私の名前は山田です。', N'私は山田です。', N'山田は私の名前です。', N'私の名前は山田。', 'A', 1.00, 'exercise', 1),
(N'Chọn cách giới thiệu quốc tịch đúng:', N'私は日本人です。', N'私は日本です。', N'日本人は私です。', N'私は日本の人です。', 'A', 1.00, 'exercise', 1),
(N'Chọn câu đúng để hỏi tên người khác:', N'お名前は何ですか？', N'あなたは誰ですか？', N'名前は何ですか？', N'あなたの名前は？', 'A', 1.00, 'exercise', 1),
(N'Dịch: "Rất vui được gặp bạn."', N'よろしくお願いします。', N'はじめまして。', N'お元気ですか？', N'ありがとう。', 'B', 1.00, 'exercise', 1);

-- Bài tập 2 (LessonId = 2): Bài tập viết Hiragana (Phần 1)
INSERT INTO Question (questionText, optionA, optionB, optionC, optionD, correctOption, mark, entityType, entityID) VALUES
(N'Chữ Hiragana nào biểu thị âm "ka"?', N'か', N'さ', N'た', N'な', 'A', 1.00, 'exercise', 2),
(N'Chữ Hiragana nào biểu thị âm "sa"?', N'さ', N'か', N'た', N'な', 'A', 1.00, 'exercise', 2),
(N'Viết Hiragana cho từ "sushi":', N'すし', N'しす', N'すさ', N'さし', 'A', 1.00, 'exercise', 2),
(N'Chọn chữ Hiragana đúng cho âm "ta":', N'た', N'な', N'か', N'さ', 'A', 1.00, 'exercise', 2),
(N'Chữ Hiragana nào biểu thị âm "na"?', N'な', N'か', N'さ', N'た', 'A', 1.00, 'exercise', 2);

-- Bài tập 3 (LessonId = 3): Bài tập viết Hiragana (Phần 2)
INSERT INTO Question (questionText, optionA, optionB, optionC, optionD, correctOption, mark, entityType, entityID) VALUES
(N'Chữ Hiragana nào biểu thị âm "ha"?', N'は', N'ま', N'や', N'ら', 'A', 1.00, 'exercise', 3),
(N'Chữ Hiragana nào biểu thị âm "ma"?', N'ま', N'は', N'や', N'ら', 'A', 1.00, 'exercise', 3),
(N'Viết Hiragana cho từ "yama" (núi):', N'やま', N'まや', N'はや', N'らや', 'A', 1.00, 'exercise', 3),
(N'Chọn chữ Hiragana đúng cho âm "ra":', N'ら', N'は', N'ま', N'や', 'A', 1.00, 'exercise', 3),
(N'Chữ Hiragana nào biểu thị âm "wa"?', N'わ', N'や', N'ま', N'ら', 'A', 1.00, 'exercise', 3);

-- Bài tập 4 (LessonId = 4): Bài tập Katakana và phát âm
INSERT INTO Question (questionText, optionA, optionB, optionC, optionD, correctOption, mark, entityType, entityID) VALUES
(N'Chữ Katakana nào biểu thị âm "a"?', N'ア', N'イ', N'ウ', N'エ', 'A', 1.00, 'exercise', 4),
(N'Viết Katakana cho từ "pizza":', N'ピザ', N'ザピ', N'ピサ', N'サピ', 'A', 1.00, 'exercise', 4),
(N'Chọn Katakana đúng cho âm "ka":', N'カ', N'サ', N'タ', N'ナ', 'A', 1.00, 'exercise', 4),
(N'Chữ Katakana nào biểu thị âm "to"?', N'ト', N'ソ', N'コ', N'ノ', 'A', 1.00, 'exercise', 4),
(N'Phát âm trường âm trong Katakana được biểu thị bằng:', N'ー', N'・', N'〜', N'．', 'A', 1.00, 'exercise', 4);

-- Bài tập 5 (LessonId = 5): Bài tập chia động từ thể Te
INSERT INTO Question (questionText, optionA, optionB, optionC, optionD, correctOption, mark, entityType, entityID) VALUES
(N'Chia động từ "taberu" (ăn) sang thể Te:', N'食べて', N'食べます', N'食べた', N'食べる', 'A', 1.00, 'exercise', 5),
(N'Chia động từ "miru" (xem) sang thể Te:', N'見て', N'見ます', N'見た', N'見る', 'A', 1.00, 'exercise', 5),
(N'Chọn câu đúng sử dụng thể Te:', N'本を読んで、寝ます。', N'本を読んだ、寝ます。', N'本を読んで、寝ました。', N'本を読む、寝ます。', 'A', 1.00, 'exercise', 5),
(N'Chia động từ "kaku" (viết) sang thể Te:', N'書いて', N'書きます', N'書いた', N'書く', 'A', 1.00, 'exercise', 5),
(N'Ý nghĩa của thể Te trong câu "行って、買う":', N'Làm liên tiếp', N'Lý do', N'Mục đích', N'Điều kiện', 'A', 1.00, 'exercise', 5);

-- Bài tập 6 (LessonId = 6): Bài tập thể Ta và diễn tả kinh nghiệm
INSERT INTO Question (questionText, optionA, optionB, optionC, optionD, correctOption, mark, entityType, entityID) VALUES
(N'Chia động từ "iku" (đi) sang thể Ta:', N'行った', N'行きます', N'行って', N'行く', 'A', 1.00, 'exercise', 6),
(N'Chọn câu đúng diễn tả kinh nghiệm:', N'日本へ行ったことがあります。', N'日本へ行きます。', N'日本へ行ってください。', N'日本へ行く。', 'A', 1.00, 'exercise', 6),
(N'Chia động từ "taberu" (ăn) sang thể Ta:', N'食べた', N'食べて', N'食べます', N'食べる', 'A', 1.00, 'exercise', 6),
(N'Dịch: "Tôi đã từng ăn sushi."', N'すしを食べたことがあります。', N'すしを食べます。', N'すしを食べました。', N'すしを食べる。', 'A', 1.00, 'exercise', 6),
(N'Ý nghĩa của cấu trúc "V-ta koto ga aru":', N'Đã từng làm gì đó', N'Sẽ làm gì đó', N'Đang làm gì đó', N'Không làm gì đó', 'A', 1.00, 'exercise', 6);

-- Bài tập 7 (LessonId = 7): Bài tập dạng ngắn (thể thông thường)
INSERT INTO Question (questionText, optionA, optionB, optionC, optionD, correctOption, mark, entityType, entityID) VALUES
(N'Chuyển câu "私は学生です。" sang thể thông thường:', N'私は学生だ。', N'私は学生です。', N'私は学生。', N'私は学生で。', 'A', 1.00, 'exercise', 7),
(N'Chia động từ "miru" (xem) sang thể thông thường:', N'見る', N'見ます', N'見た', N'見て', 'A', 1.00, 'exercise', 7),
(N'Chọn câu đúng ở thể thông thường:', N'本が好きだ。', N'本が好きです。', N'本が好きでした。', N'本が好きで。', 'A', 1.00, 'exercise', 7),
(N'Chuyển tính từ "takai" (đắt) sang thể thông thường:', N'高い', N'高く', N'高かった', N'高くて', 'A', 1.00, 'exercise', 7),
(N'Ý nghĩa của thể thông thường:', N'Thân mật, không lịch sự', N'Lịch sự, trang trọng', N'Diễn tả quá khứ', N'Diễn tả điều kiện', 'A', 1.00, 'exercise', 7);

-- Bài tập 8 (LessonId = 8): Bài tập cấu trúc câu cơ bản
INSERT INTO Question (questionText, optionA, optionB, optionC, optionD, correctOption, mark, entityType, entityID) VALUES
(N'Chọn cấu trúc câu đúng:', N'私は本を読む。', N'本を私は読む。', N'読む本を私は。', N'私は読む本を。', 'A', 1.00, 'exercise', 8),
(N'Trợ từ nào phù hợp trong câu "学校___行きます。":', N'へ', N'に', N'で', N'と', 'A', 1.00, 'exercise', 8),
(N'Chọn câu đúng:', N'猫が好きです。', N'猫を好きです。', N'猫は好きです。', N'猫が好き。', 'A', 1.00, 'exercise', 8),
(N'Dịch: "Tôi đi đến Tokyo."', N'私は東京へ行きます。', N'私は東京をします。', N'東京は私が行きます。', N'私は東京で。', 'A', 1.00, 'exercise', 8),
(N'Trợ từ nào dùng để chỉ đối tượng trong câu?', N'を', N'に', N'で', N'へ', 'A', 1.00, 'exercise', 8);

-- Bài tập 9 (LessonId = 9): Bài tập sử dụng trợ từ
INSERT INTO Question (questionText, optionA, optionB, optionC, optionD, correctOption, mark, entityType, entityID) VALUES
(N'Trợ từ nào phù hợp: "図書館___本を借ります。":', N'で', N'に', N'へ', N'を', 'A', 1.00, 'exercise', 9),
(N'Chọn câu đúng: "友達___手紙を書きます。":', N'に', N'で', N'へ', N'と', 'A', 1.00, 'exercise', 9),
(N'Trợ từ "から" có ý nghĩa gì?', N'Từ (nơi xuất phát)', N'Đến', N'Tại', N'Cùng với', 'A', 1.00, 'exercise', 9),
(N'Chọn câu đúng: "学校___家まで歩きます。":', N'から', N'に', N'で', N'へ', 'A', 1.00, 'exercise', 9),
(N'Trợ từ nào chỉ nơi thực hiện hành động?', N'で', N'に', N'へ', N'と', 'A', 1.00, 'exercise', 9);

-- Bài tập 10 (LessonId = 10): Bài tập biểu hiện sự tồn tại
INSERT INTO Question (questionText, optionA, optionB, optionC, optionD, correctOption, mark, entityType, entityID) VALUES
(N'Chọn câu đúng diễn tả sự tồn tại của người:', N'教室に学生がいます。', N'教室に学生があります。', N'教室で学生がいます。', N'教室に学生です。', 'A', 1.00, 'exercise', 10),
(N'Chọn động từ đúng cho vật vô tri:', N'あります', N'います', N'です', N'でした', 'A', 1.00, 'exercise', 10),
(N'Dịch: "Có một con mèo trong vườn."', N'庭に猫がいます。', N'庭に猫があります。', N'庭で猫がいます。', N'庭に猫です。', 'A', 1.00, 'exercise', 10),
(N'Chọn câu đúng: "Có sách trên bàn."', N'机に本があります。', N'机に本がいます。', N'机で本があります。', N'机に本です。', 'A', 1.00, 'exercise', 10),
(N'Trợ từ nào thường dùng với "imasu" hoặc "arimasu"?', N'に', N'で', N'へ', N'と', 'A', 1.00, 'exercise', 10);

-- Mẫu chung cho các bài tập còn lại (Exercise 11 đến 36)
-- Để tiết kiệm không gian, tôi cung cấp mẫu câu hỏi cho các bài tập còn lại.
-- Bạn có thể yêu cầu chi tiết nếu cần.

-- Bài tập 11 (LessonId = 11): Bài tập nhận diện bộ thủ Kanji
INSERT INTO Question (questionText, optionA, optionB, optionC, optionD, correctOption, mark, entityType, entityID) VALUES
(N'Bộ thủ nào có nghĩa là "nước"?', N'氵', N'木', N'人', N'日', 'A', 1.00, 'exercise', 11),
(N'Bộ thủ nào có nghĩa là "cây"?', N'木', N'氵', N'人', N'日', 'A', 1.00, 'exercise', 11),
(N'Kanji "川" thuộc bộ thủ nào?', N'氵', N'木', N'人', N'日', 'A', 1.00, 'exercise', 11),
(N'Bộ thủ nào có nghĩa là "người"?', N'人', N'木', N'氵', N'日', 'A', 1.00, 'exercise', 11),
(N'Chọn Kanji đúng có bộ thủ "日":', N'時', N'川', N'林', N'人', 'A', 1.00, 'exercise', 11);

-- Bài tập 12 (LessonId = 12): Bài tập Kanji số đếm và thời gian
INSERT INTO Question (questionText, optionA, optionB, optionC, optionD, correctOption, mark, entityType, entityID) VALUES
(N'Kanji nào biểu thị số "một"?', N'一', N'二', N'三', N'四', 'A', 1.00, 'exercise', 12),
(N'Kanji nào biểu thị "tháng"?', N'月', N'日', N'年', N'時', 'A', 1.00, 'exercise', 12),
(N'Viết Kanji cho "năm":', N'年', N'月', N'日', N'時', 'A', 1.00, 'exercise', 12),
(N'Kanji nào biểu thị "thứ hai"?', N'月', N'火', N'水', N'木', 'A', 1.00, 'exercise', 12),
(N'Dịch: "Ba ngày" bằng Kanji:', N'三日', N'三月', N'三年', N'三時', 'A', 1.00, 'exercise', 12);

-- Bài tập 13 (LessonId = 13): Bài tập Kanji về con người
INSERT INTO Question (questionText, optionA, optionB, optionC, optionD, correctOption, mark, entityType, entityID) VALUES
(N'Kanji nào biểu thị "người"?', N'人', N'子', N'父', N'母', 'A', 1.00, 'exercise', 13),
(N'Kanji nào biểu thị "cha"?', N'父', N'母', N'子', N'人', 'A', 1.00, 'exercise', 13),
(N'Viết Kanji cho "mẹ":', N'母', N'父', N'子', N'人', 'A', 1.00, 'exercise', 13),
(N'Kanji nào biểu thị "giáo viên"?', N'先生', N'学生', N'会社', N'学校', 'A', 1.00, 'exercise', 13),
(N'Dịch: "Học sinh" bằng Kanji:', N'学生', N'先生', N'会社', N'学校', 'A', 1.00, 'exercise', 13);

-- Bài tập 14 (LessonId = 14): Bài tập hội thoại giới thiệu
INSERT INTO Question (questionText, optionA, optionB, optionC, optionD, correctOption, mark, entityType, entityID) VALUES
(N'Chọn câu đúng để giới thiệu bản thân:', N'はじめまして、田中です。', N'お元気ですか？', N'ありがとう。', N'さようなら。', 'A', 1.00, 'exercise', 14),
(N'Dịch: "Tôi đến từ Việt Nam."', N'私はベトナムから来ました。', N'私はベトナムです。', N'ベトナムは私です。', N'私はベトナムにいます。', 'A', 1.00, 'exercise', 14),
(N'Chọn câu hỏi đúng để hỏi nghề nghiệp:', N'お仕事は何ですか？', N'お名前は何ですか？', N'どこに住んでいますか？', N'いくつですか？', 'A', 1.00, 'exercise', 14),
(N'Chọn câu trả lời phù hợp cho "お名前は何ですか？":', N'山田です。', N'日本です。', N'学生です。', N'20歳です。', 'A', 1.00, 'exercise', 14),
(N'Dịch: "Rất vui được gặp bạn."', N'はじめまして。', N'お元気ですか？', N'ありがとう。', N'さようなら。', 'A', 1.00, 'exercise', 14);

-- Bài tập 15 (LessonId = 15): Bài tập nghe thông báo
INSERT INTO Question (questionText, optionA, optionB, optionC, optionD, correctOption, mark, entityType, entityID) VALUES
(N'Nghe thông báo: "電車が遅れます。" có nghĩa là gì?', N'Tàu bị trễ.', N'Tàu đến sớm.', N'Tàu bị hủy.', N'Tàu đang chạy.', 'A', 1.00, 'exercise', 15),
(N'Nghe: "店は10時に開きます。" có nghĩa là gì?', N'Cửa hàng mở lúc 10 giờ.', N'Cửa hàng đóng lúc 10 giờ.', N'Cửa hàng mở lúc 12 giờ.', N'Cửa hàng mở cả ngày.', 'A', 1.00, 'exercise', 15),
(N'Nghe: "バスは5分後に来ます。" có nghĩa là gì?', N'Xe buýt đến sau 5 phút.', N'Xe buýt đến sau 10 phút.', N'Xe buýt đã đi.', N'Xe buýt bị hủy.', 'A', 1.00, 'exercise', 15),
(N'Nghe: "図書館は静かにしてください。" có nghĩa là gì?', N'Hãy giữ yên lặng trong thư viện.', N'Hãy đọc sách trong thư viện.', N'Hãy mượn sách trong thư viện.', N'Hãy đóng cửa thư viện.', 'A', 1.00, 'exercise', 15),
(N'Nghe: "会議は2時から始まります。" có nghĩa là gì?', N'Cuộc họp bắt đầu lúc 2 giờ.', N'Cuộc họp kết thúc lúc 2 giờ.', N'Cuộc họp bị hủy.', N'Cuộc họp bắt đầu lúc 3 giờ.', 'A', 1.00, 'exercise', 15);

-- Bài tập 16 (LessonId = 16): Bài tập đặt câu hỏi
INSERT INTO Question (questionText, optionA, optionB, optionC, optionD, correctOption, mark, entityType, entityID) VALUES
(N'Chọn câu hỏi đúng để hỏi tuổi:', N'いくつですか？', N'お名前は何ですか？', N'どこに住んでいますか？', N'お仕事は何ですか？', 'A', 1.00, 'exercise', 16),
(N'Dịch: "Bạn sống ở đâu?"', N'どこに住んでいますか？', N'いくつですか？', N'お名前は何ですか？', N'お仕事は何ですか？', 'A', 1.00, 'exercise', 16),
(N'Chọn câu trả lời phù hợp cho "いくつですか？":', N'20歳です。', N'東京です。', N'山田です。', N'学生です。', 'A', 1.00, 'exercise', 16),
(N'Chọn câu hỏi đúng để hỏi sở thích:', N'何が好きですか？', N'どこにいますか？', N'誰ですか？', N'いくつですか？', 'A', 1.00, 'exercise', 16),
(N'Dịch: "Bạn làm nghề gì?"', N'お仕事は何ですか？', N'お名前は何ですか？', N'どこに住んでいますか？', N'いくつですか？', 'A', 1.00, 'exercise', 16);

-- Bài tập 17 (LessonId = 17): Bài tập nhận diện Kanji cơ bản
INSERT INTO Question (questionText, optionA, optionB, optionC, optionD, correctOption, mark, entityType, entityID) VALUES
(N'Kanji nào biểu thị "núi"?', N'山', N'川', N'木', N'田', 'A', 1.00, 'exercise', 17),
(N'Kanji nào biểu thị "sông"?', N'川', N'山', N'木', N'田', 'A', 1.00, 'exercise', 17),
(N'Chọn Kanji đúng cho "cây":', N'木', N'山', N'川', N'田', 'A', 1.00, 'exercise', 17),
(N'Kanji nào biểu thị "điền/ruộng"?', N'田', N'山', N'川', N'木', 'A', 1.00, 'exercise', 17),
(N'Dịch: "Rừng" bằng Kanji:', N'林', N'山', N'川', N'田', 'A', 1.00, 'exercise', 17);

-- Bài tập 18 (LessonId = 18): Bài tập Kanji về thiên nhiên
INSERT INTO Question (questionText, optionA, optionB, optionC, optionD, correctOption, mark, entityType, entityID) VALUES
(N'Kanji nào biểu thị "hoa"?', N'花', N'木', N'山', N'川', 'A', 1.00, 'exercise', 18),
(N'Kanji nào biểu thị "biển"?', N'海', N'川', N'山', N'木', 'A', 1.00, 'exercise', 18),
(N'Chọn Kanji đúng cho "mưa":', N'雨', N'花', N'海', N'木', 'A', 1.00, 'exercise', 18),
(N'Kanji nào biểu thị "trời"?', N'空', N'海', N'花', N'雨', 'A', 1.00, 'exercise', 18),
(N'Dịch: "Đồi" bằng Kanji:', N'丘', N'山', N'海', N'花', 'A', 1.00, 'exercise', 18);

-- Bài tập 19 (LessonId = 19): Bài tập Kanji về con người và gia đình
INSERT INTO Question (questionText, optionA, optionB, optionC, optionD, correctOption, mark, entityType, entityID) VALUES
(N'Kanji nào biểu thị "anh trai"?', N'兄', N'姉', N'弟', N'妹', 'A', 1.00, 'exercise', 19),
(N'Kanji nào biểu thị "chị gái"?', N'姉', N'兄', N'弟', N'妹', 'A', 1.00, 'exercise', 19),
(N'Chọn Kanji đúng cho "em trai":', N'弟', N'兄', N'姉', N'妹', 'A', 1.00, 'exercise', 19),
(N'Kanji nào biểu thị "em gái"?', N'妹', N'兄', N'姉', N'弟', 'A', 1.00, 'exercise', 19),
(N'Dịch: "Gia đình" bằng Kanji:', N'家族', N'親子', N'兄弟', N'姉妹', 'A', 1.00, 'exercise', 19);

-- Bài tập 20 (LessonId = 20): Bài tập đặt phòng và hỏi đường
INSERT INTO Question (questionText, optionA, optionB, optionC, optionD, correctOption, mark, entityType, entityID) VALUES
(N'Chọn câu đúng để hỏi đường đến ga:', N'駅はどこですか？', N'ホテルはどこですか？', N'トイレはどこですか？', N'お店はどこですか？', 'A', 1.00, 'exercise', 20),
(N'Dịch: "Khách sạn ở đâu?"', N'ホテルはどこですか？', N'駅はどこですか？', N'トイレはどこですか？', N'お店はどこですか？', 'A', 1.00, 'exercise', 20),
(N'Chọn câu đúng để đặt phòng:', N'部屋を予約したいです。', N'部屋を借りたいです。', N'部屋を貸してください。', N'部屋を見たいです。', 'A', 1.00, 'exercise', 20),
(N'Dịch: "Tôi muốn đặt một phòng đơn."', N'シングルルームを予約したいです。', N'ダブルルームを予約したいです。', N'部屋を借りたいです。', N'部屋を見たいです。', 'A', 1.00, 'exercise', 20),
(N'Chọn câu trả lời phù hợp khi được hỏi đường:', N'まっすぐ行って、右に曲がります。', N'ありがとうございます。', N'すみません。', N'よろしくお願いします。', 'A', 1.00, 'exercise', 20);

-- Bài tập 21 (LessonId = 21): Bài tập giao tiếp tại nhà hàng
INSERT INTO Question (questionText, optionA, optionB, optionC, optionD, correctOption, mark, entityType, entityID) VALUES
(N'Chọn câu đúng để gọi món ăn:', N'メニューを見せてください。', N'お勘定をお願いします。', N'水をください。', N'これをください。', 'D', 1.00, 'exercise', 21),
(N'Dịch: "Tôi muốn một cốc nước."', N'水をください。', N'メニューを見せてください。', N'お勘定をお願いします。', N'これをください。', 'A', 1.00, 'exercise', 21),
(N'Chọn câu đúng để hỏi giá:', N'いくらですか？', N'何ですか？', N'どこですか？', N'誰ですか？', 'A', 1.00, 'exercise', 21),
(N'Dịch: "Thanh toán, làm ơn."', N'お勘定をお願いします。', N'メニューを見せてください。', N'水をください。', N'これをください。', 'A', 1.00, 'exercise', 21),
(N'Chọn câu đúng để yêu cầu thực đơn:', N'メニューを見せてください。', N'お勘定をお願いします。', N'水をください。', N'これをください。', 'A', 1.00, 'exercise', 21);

-- Bài tập 22 (LessonId = 22): Bài tập xử lý tình huống khẩn cấp
INSERT INTO Question (questionText, optionA, optionB, optionC, optionD, correctOption, mark, entityType, entityID) VALUES
(N'Chọn câu đúng khi cần gọi bác sĩ:', N'医者を呼んでください。', N'警察を呼んでください。', N'消防車を呼んでください。', N'救急車を呼んでください。', 'A', 1.00, 'exercise', 22),
(N'Dịch: "Tôi bị đau bụng."', N'お腹が痛いです。', N'頭が痛いです。', N'足が痛いです。', N'手が痛いです。', 'A', 1.00, 'exercise', 22),
(N'Chọn câu đúng để gọi cứu hỏa:', N'消防車を呼んでください。', N'医者を呼んでください。', N'警察を呼んでください。', N'救急車を呼んでください。', 'A', 1.00, 'exercise', 22),
(N'Dịch: "Cứu tôi với!"', N'助けてください！', N'ありがとう！', N'すみません！', N'お願いします！', 'A', 1.00, 'exercise', 22),
(N'Chọn câu đúng khi cần gọi cảnh sát:', N'警察を呼んでください。', N'医者を呼んでください。', N'消防車を呼んでください。', N'救急車を呼んでください。', 'A', 1.00, 'exercise', 22);

-- Bài tập 23 (LessonId = 23): Bài tập viết email kinh doanh
INSERT INTO Question (questionText, optionA, optionB, optionC, optionD, correctOption, mark, entityType, entityID) VALUES
(N'Chọn câu mở đầu đúng cho email kinh doanh:', N'お世話になっております。', N'こんにちは。', N'はじめまして。', N'ありがとうございます。', 'A', 1.00, 'exercise', 23),
(N'Dịch: "Kính gửi ông Tanaka."', N'田中様へ', N'田中さんへ', N'田中くんへ', N'田中へ', 'A', 1.00, 'exercise', 23),
(N'Chọn câu kết thúc email lịch sự:', N'よろしくお願いいたします。', N'ありがとうございます。', N'さようなら。', N'お元気ですか？', 'A', 1.00, 'exercise', 23),
(N'Chọn câu đúng để yêu cầu lịch hẹn:', N'ご都合のよい時間をお知らせください。', N'お名前を教えてください。', N'お元気ですか？', N'ありがとうございます。', 'A', 1.00, 'exercise', 23),
(N'Dịch: "Cảm ơn sự hợp tác của quý công ty."', N'ご協力ありがとうございます。', N'お世話になっております。', N'よろしくお願いいたします。', N'はじめまして。', 'A', 1.00, 'exercise', 23);

-- Bài tập 24 (LessonId = 24): Bài tập kính ngữ cơ bản
INSERT INTO Question (questionText, optionA, optionB, optionC, optionD, correctOption, mark, entityType, entityID) VALUES
(N'Chọn dạng kính ngữ của động từ "行く":', N'いらっしゃる', N'行く', N'行きます', N'行った', 'A', 1.00, 'exercise', 24),
(N'Chọn dạng kính ngữ của động từ "食べる":', N'召し上がる', N'食べます', N'食べた', N'食べる', 'A', 1.00, 'exercise', 24),
(N'Chọn câu đúng sử dụng kính ngữ:', N'先生がいらっしゃいます。', N'先生が行きます。', N'先生がいます。', N'先生が行った。', 'A', 1.00, 'exercise', 24),
(N'Dịch: "Xin mời ngồi."', N'お座りください。', N'座ってください。', N'座ります。', N'座る。', 'A', 1.00, 'exercise', 24),
(N'Chọn dạng kính ngữ của "見る":', N'ご覧になる', N'見ます', N'見た', N'見る', 'A', 1.00, 'exercise', 24);

-- Bài tập 25 (LessonId = 25): Bài tập thảo luận công việc
INSERT INTO Question (questionText, optionA, optionB, optionC, optionD, correctOption, mark, entityType, entityID) VALUES
(N'Chọn câu đúng để bắt đầu thảo luận:', N'ご意見を伺いたいと思います。', N'ありがとうございます。', N'お元気ですか？', N'さようなら。', 'A', 1.00, 'exercise', 25),
(N'Dịch: "Tôi muốn đề xuất một ý tưởng."', N'提案があります。', N'質問があります。', N'お願いがあります。', N'問題があります。', 'A', 1.00, 'exercise', 25),
(N'Chọn câu đúng để đồng ý trong họp:', N'賛成です。', N'反対です。', N'問題です。', N'質問です。', 'A', 1.00, 'exercise', 25),
(N'Dịch: "Chúng ta nên xem xét chi phí."', N'費用を検討すべきです。', N'費用を払うべきです。', N'費用を借ります。', N'費用を計算します。', 'A', 1.00, 'exercise', 25),
(N'Chọn câu đúng để yêu cầu ý kiến:', N'ご意見を教えてください。', N'ご質問を教えてください。', N'ご提案を教えてください。', N'ご問題を教えてください。', 'A', 1.00, 'exercise', 25);

-- Bài tập 26 (LessonId = 26): Bài tập đọc hiểu báo chí
INSERT INTO Question (questionText, optionA, optionB, optionC, optionD, correctOption, mark, entityType, entityID) VALUES
(N'Chọn từ đúng điền vào chỗ trống: 「天気が___、外出します。」', N'よければ', N'悪ければ', N'よかったら', N'悪いなら', 'A', 1.00, 'exercise', 26),
(N'Dịch: "Thời tiết hôm nay rất đẹp."', N'今日の天気はとてもいいです。', N'今日の天気は悪いです。', N'今日の天気は寒いです。', N'今日の天気は暑いです。', 'A', 1.00, 'exercise', 26),
(N'Chọn ý chính của đoạn văn: 「日本で地震が発生しました。」', N'Có động đất ở Nhật Bản.', N'Có bão ở Nhật Bản.', N'Có mưa lớn ở Nhật Bản.', N'Có tuyết ở Nhật Bản.', 'A', 1.00, 'exercise', 26),
(N'Chọn từ đồng nghĩa với "新聞" (báo):', N'ニュース', N'テレビ', N'ラジオ', N'インターネット', 'A', 1.00, 'exercise', 26),
(N'Dịch: "Bài báo viết về kinh tế."', N'新聞は経済について書いています。', N'新聞は天気について書いています。', N'新聞はスポーツについて書いています。', N'新聞は文化について書いています。', 'A', 1.00, 'exercise', 26);

-- Bài tập 27 (LessonId = 27): Bài tập phân tích văn bản học thuật
INSERT INTO Question (questionText, optionA, optionB, optionC, optionD, correctOption, mark, entityType, entityID) VALUES
(N'Chọn từ đúng điền vào: 「研究は___を目的とします。」', N'発展', N'失敗', N'停止', N'遅れ', 'A', 1.00, 'exercise', 27),
(N'Dịch: "Nghiên cứu này rất quan trọng."', N'この研究はとても重要です。', N'この研究は簡単です。', N'この研究は難しいです。', N'この研究は短いです。', 'A', 1.00, 'exercise', 27),
(N'Chọn từ đồng nghĩa với "研究" (nghiên cứu):', N'調査', N'実験', N'分析', N'報告', 'A', 1.00, 'exercise', 27),
(N'Chọn câu đúng trong văn bản học thuật:', N'データに基づいて結論を出します。', N'データを見ます。', N'データを作ります。', N'データを借ります。', 'A', 1.00, 'exercise', 27),
(N'Dịch: "Phân tích dữ liệu là cần thiết."', N'データの分析が必要です。', N'データの作成が必要です。', N'データの収集が必要です。', N'データの確認が必要です。', 'A', 1.00, 'exercise', 27);

-- Bài tập 28 (LessonId = 28): Bài tập luyện thi đọc hiểu JLPT N3
INSERT INTO Question (questionText, optionA, optionB, optionC, optionD, correctOption, mark, entityType, entityID) VALUES
(N'Chọn từ đúng điền vào: 「彼は忙しくて___。」', N'来られない', N'来ます', N'来た', N'来る', 'A', 1.00, 'exercise', 28),
(N'Dịch: "Tôi không thể đến vì bận."', N'忙しくて来られません。', N'忙しくて来ます。', N'忙しくて来た。', N'忙しくて来る。', 'A', 1.00, 'exercise', 28),
(N'Chọn câu đúng theo ngữ cảnh JLPT N3:', N'天気がよければ、行きます。', N'天気がよかった、行きます。', N'天気がよければ、行った。', N'天気がよい、行きます。', 'A', 1.00, 'exercise', 28),
(N'Chọn từ đồng nghĩa với "簡単" (dễ):', N'やさしい', N'難しい', N'高い', N'低い', 'A', 1.00, 'exercise', 28),
(N'Dịch: "Bài kiểm tra này rất dễ."', N'このテストはやさしいです。', N'このテストは難しいです。', N'このテストは高いです。', N'このテストは低いです。', 'A', 1.00, 'exercise', 28);

-- Bài tập 29 (LessonId = 29): Bài tập ngữ pháp N4
INSERT INTO Question (questionText, optionA, optionB, optionC, optionD, correctOption, mark, entityType, entityID) VALUES
(N'Chọn cấu trúc đúng cho "V-nai de kudasai":', N'食べないでください。', N'食べてください。', N'食べました。', N'食べる。', 'A', 1.00, 'exercise', 29),
(N'Dịch: "Đừng chạy."', N'走らないでください。', N'走ってください。', N'走りました。', N'走る。', 'A', 1.00, 'exercise', 29),
(N'Chọn câu đúng sử dụng "te mo ii":', N'食べてもいいです。', N'食べないでください。', N'食べます。', N'食べました。', 'A', 1.00, 'exercise', 29),
(N'Chọn cấu trúc đúng cho "V-tai":', N'食べたい', N'食べます', N'食べました', N'食べる', 'A', 1.00, 'exercise', 29),
(N'Dịch: "Tôi muốn uống trà."', N'お茶を飲みたい。', N'お茶を飲みます。', N'お茶を飲みました。', N'お茶を飲む。', 'A', 1.00, 'exercise', 29);

-- Bài tập 30 (LessonId = 30): Bài tập từ vựng N4
INSERT INTO Question (questionText, optionA, optionB, optionC, optionD, correctOption, mark, entityType, entityID) VALUES
(N'Chọn từ đúng cho "bệnh viện":', N'病院', N'学校', N'会社', N'銀行', 'A', 1.00, 'exercise', 30),
(N'Chọn từ đúng cho "xe đạp":', N'自転車', N'車', N'電車', N'バス', 'A', 1.00, 'exercise', 30),
(N'Dịch: "Cửa hàng tiện lợi" bằng Kanji:', N'コンビニ', N'スーパー', N'デパート', N'病院', 'A', 1.00, 'exercise', 30),
(N'Chọn từ đúng cho "ngày mai":', N'明日', N'今日', N'昨日', N'今週', 'A', 1.00, 'exercise', 30),
(N'Chọn từ đúng cho "đẹp":', N'きれい', N'かわいい', N'おいしい', N'たかい', 'A', 1.00, 'exercise', 30);

-- Bài tập 31 (LessonId = 31): Bài tập nghe và đọc hiểu N4
INSERT INTO Question (questionText, optionA, optionB, optionC, optionD, correctOption, mark, entityType, entityID) VALUES
(N'Nghe: "店は5時に閉まります。" có nghĩa là gì?', N'Cửa hàng đóng lúc 5 giờ.', N'Cửa hàng mở lúc 5 giờ.', N'Cửa hàng đóng lúc 6 giờ.', N'Cửa hàng mở cả ngày.', 'A', 1.00, 'exercise', 31),
(N'Nghe: "バスは10分後に来ます。" có nghĩa là gì?', N'Xe buýt đến sau 10 phút.', N'Xe buýt đến sau 5 phút.', N'Xe buýt đã đi.', N'Xe buýt bị hủy.', 'A', 1.00, 'exercise', 31),
(N'Đọc: "私は毎日学校へ行きます。" có nghĩa là gì?', N'Tôi đi học mỗi ngày.', N'Tôi đi làm mỗi ngày.', N'Tôi đi chơi mỗi ngày.', N'Tôi ở nhà mỗi ngày.', 'A', 1.00, 'exercise', 31),
(N'Nghe: "会議は3時からです。" có nghĩa là gì?', N'Cuộc họp bắt đầu lúc 3 giờ.', N'Cuộc họp kết thúc lúc 3 giờ.', N'Cuộc họp bị hủy.', N'Cuộc họp bắt đầu lúc 4 giờ.', 'A', 1.00, 'exercise', 31),
(N'Đọc: "これは私の本です。" có nghĩa là gì?', N'Đây là sách của tôi.', N'Đây là sách của bạn.', N'Đây là bút của tôi.', N'Đây là bàn của tôi.', 'A', 1.00, 'exercise', 31);

-- Bài tập 32 (LessonId = 32): Bài tập ngữ pháp N3 nâng cao
INSERT INTO Question (questionText, optionA, optionB, optionC, optionD, correctOption, mark, entityType, entityID) VALUES
(N'Chọn cấu trúc đúng cho "V-nagara":', N'歩きながら話す。', N'歩いて話す。', N'歩いた話す。', N'歩く話す。', 'A', 1.00, 'exercise', 32),
(N'Dịch: "Tôi vừa đi vừa nghe nhạc."', N'歩きながら音楽を聞く。', N'歩いて音楽を聞く。', N'歩いた音楽を聞く。', N'歩く音楽を聞く。', 'A', 1.00, 'exercise', 32),
(N'Chọn câu đúng sử dụng "tari":', N'食べたり飲んだりします。', N'食べて飲む。', N'食べた飲んだ。', N'食べる飲む。', 'A', 1.00, 'exercise', 32),
(N'Chọn cấu trúc đúng cho "V-ba ii":', N'行けばいい。', N'行くといい。', N'行ったいい。', N'行くいい。', 'A', 1.00, 'exercise', 32),
(N'Dịch: "Nếu trời đẹp, chúng ta nên đi dạo."', N'天気がよければ、散歩すればいい。', N'天気がよかった、散歩する。', N'天気がよい、散歩した。', N'天気がよければ、散歩する。', 'A', 1.00, 'exercise', 32);

-- Bài tập 33 (LessonId = 33): Bài tập Kanji và từ vựng N3
INSERT INTO Question (questionText, optionA, optionB, optionC, optionD, correctOption, mark, entityType, entityID) VALUES
(N'Kanji nào biểu thị "ý kiến"?', N'意見', N'質問', N'提案', N'問題', 'A', 1.00, 'exercise', 33),
(N'Chọn từ đúng cho "giao thông":', N'交通', N'経済', N'政治', N'文化', 'A', 1.00, 'exercise', 33),
(N'Kanji nào biểu thị "kế hoạch"?', N'計画', N'意見', N'質問', N'提案', 'A', 1.00, 'exercise', 33),
(N'Dịch: "Giáo dục" bằng Kanji:', N'教育', N'文化', N'経済', N'政治', 'A', 1.00, 'exercise', 33),
(N'Chọn từ đúng cho "môi trường":', N'環境', N'経済', N'交通', N'文化', 'A', 1.00, 'exercise', 33);

-- Bài tập 34 (LessonId = 34): Bài tập nghe và đọc hiểu N3
INSERT INTO Question (questionText, optionA, optionB, optionC, optionD, correctOption, mark, entityType, entityID) VALUES
(N'Nghe: "会議は明日です。" có nghĩa là gì?', N'Cuộc họp diễn ra ngày mai.', N'Cuộc họp diễn ra hôm nay.', N'Cuộc họp bị hủy.', N'Cuộc họp diễn ra tuần sau.', 'A', 1.00, 'exercise', 34),
(N'Đọc: "彼は毎日走ります。" có nghĩa là gì?', N'Anh ấy chạy mỗi ngày.', N'Anh ấy chạy mỗi tuần.', N'Anh ấy chạy mỗi tháng.', N'Anh ấy không chạy.', 'A', 1.00, 'exercise', 34),
(N'Nghe: "店は8時に開きます。" có nghĩa là gì?', N'Cửa hàng mở lúc 8 giờ.', N'Cửa hàng đóng lúc 8 giờ.', N'Cửa hàng mở lúc 9 giờ.', N'Cửa hàng đóng cả ngày.', 'A', 1.00, 'exercise', 34),
(N'Đọc: "私は友達に会います。" có nghĩa là gì?', N'Tôi gặp bạn bè.', N'Tôi gặp gia đình.', N'Tôi gặp giáo viên.', N'Tôi gặp đồng nghiệp.', 'A', 1.00, 'exercise', 34),
(N'Nghe: "電車は5分後に来ます。" có nghĩa là gì?', N'Tàu đến sau 5 phút.', N'Tàu đến sau 10 phút.', N'Tàu đã đi.', N'Tàu bị hủy.', 'A', 1.00, 'exercise', 34);

-- Bài tập 35 (LessonId = 35): Bài tập ngữ pháp N2/N1
INSERT INTO Question (questionText, optionA, optionB, optionC, optionD, correctOption, mark, entityType, entityID) VALUES
(N'Chọn cấu trúc đúng cho "V-temo":', N'行ってもいい。', N'行くといい。', N'行ったいい。', N'行くいい。', 'A', 1.00, 'exercise', 35),
(N'Dịch: "Dù có cố gắng, tôi vẫn không hiểu."', N'頑張っても分かりません。', N'頑張って分かります。', N'頑張った分かります。', N'頑張る分かります。', 'A', 1.00, 'exercise', 35),
(N'Chọn câu đúng sử dụng "V-nakereba naranai":', N'行かなければならない。', N'行くなければならない。', N'行ったなければならない。', N'行ってなければならない。', 'A', 1.00, 'exercise', 35),
(N'Chọn cấu trúc đúng cho "V-tari":', N'食べたり飲んだりする。', N'食べて飲む。', N'食べた飲んだ。', N'食べる飲む。', 'A', 1.00, 'exercise', 35),
(N'Dịch: "Tôi phải học mỗi ngày."', N'毎日勉強しなければならない。', N'毎日勉強します。', N'毎日勉強しました。', N'毎日勉強する。', 'A', 1.00, 'exercise', 35);

-- Bài tập 36 (LessonId = 36): Bài tập đọc hiểu văn học
INSERT INTO Question (questionText, optionA, optionB, optionC, optionD, correctOption, mark, entityType, entityID) VALUES
(N'Chọn từ đúng điền vào: 「彼は___に住んでいます。」', N'田舎', N'都会', N'学校', N'会社', 'A', 1.00, 'exercise', 36),
(N'Dịch: "Câu chuyện này rất cảm động."', N'この話はとても感動的です。', N'この話はとても面白いです。', N'この話はとても難しいです。', N'この話はとても簡単です。', 'A', 1.00, 'exercise', 36),
(N'Chọn từ đồng nghĩa với "物語" (câu chuyện):', N'話', N'本', N'新聞', N'記事', 'A', 1.00, 'exercise', 36),
(N'Chọn câu đúng trong văn học:', N'彼の人生は波乱に満ちていた。', N'彼の人生は簡単だった。', N'彼の人生は短かった。', N'彼の人生は普通だった。', 'A', 1.00, 'exercise', 36),
(N'Dịch: "Tiểu thuyết này rất nổi tiếng."', N'この小説はとても有名です。', N'この小説はとても簡単です。', N'この小説はとても難しいです。', N'この小説はとても短いです。', 'A', 1.00, 'exercise', 36);

-- Bài tập 37 (LessonId = 37): Bài tập thảo luận bằng tiếng Nhật
INSERT INTO Question (questionText, optionA, optionB, optionC, optionD, correctOption, mark, entityType, entityID) VALUES
(N'Chọn câu đúng để bắt đầu thảo luận:', N'この問題についてどう思いますか？', N'ありがとうございます。', N'お元気ですか？', N'さようなら。', 'A', 1.00, 'exercise', 37),
(N'Dịch: "Bạn nghĩ gì về vấn đề này?"', N'この問題についてどう思いますか？', N'この問題を解決しますか？', N'この問題は難しいですか？', N'この問題は簡単ですか？', 'A', 1.00, 'exercise', 37),
(N'Chọn câu đúng để bày tỏ ý kiến:', N'私はこう思います。', N'私はこうします。', N'私はこうでした。', N'私はこう。', 'A', 1.00, 'exercise', 37),
(N'Dịch: "Tôi đồng ý với ý kiến của bạn."', N'あなたの意見に賛成です。', N'あなたの意見に反対です。', N'あなたの意見は難しいです。', N'あなたの意見は簡単です。', 'A', 1.00, 'exercise', 37),
(N'Chọn câu đúng để kết thúc thảo luận:', N'ご意見ありがとうございます。', N'お元気ですか？', N'さようなら。', N'はじめまして。', 'A', 1.00, 'exercise', 37);