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