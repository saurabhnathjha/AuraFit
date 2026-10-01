-- Drop the database if it exists
IF EXISTS (SELECT name FROM sys.databases WHERE name = N'AuraFitDb')
BEGIN
    ALTER DATABASE [AuraFitDb] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE [AuraFitDb];
END
GO

-- Create a fresh database
CREATE DATABASE [AuraFitDb];
GO

-- Use the new database
USE [AuraFitDb];
GO

-- Create Users first (required by other tables)
CREATE TABLE [dbo].[Users] (
    [UserId]       INT            IDENTITY (1, 1) NOT NULL,
    [Username]     NVARCHAR (50)  NOT NULL,
    [PasswordHash] NVARCHAR (256) NOT NULL,
    [Email]        NVARCHAR (100) NULL,
    [CreatedAt]    DATETIME       DEFAULT (getdate()) NULL,
    PRIMARY KEY CLUSTERED ([UserId] ASC),
    UNIQUE NONCLUSTERED ([Username] ASC)
);
GO

CREATE TABLE [dbo].[ExerciseCatalog] (
    [ExerciseCatalogId] INT            IDENTITY (1, 1) NOT NULL,
    [WgerId]            INT            NULL,
    [Name]              NVARCHAR (100) NOT NULL,
    [Category]          NVARCHAR (50)  NULL,
    [Description]       NVARCHAR (MAX) NULL,
    [Equipment]         NVARCHAR (255) NULL,
    [ImageUrl]          NVARCHAR (255) NULL,
    [METValue]          FLOAT (53)     NULL,
    PRIMARY KEY CLUSTERED ([ExerciseCatalogId] ASC)
);
GO

CREATE TABLE [dbo].[WorkoutTemplate] (
    [WorkoutTemplateId] INT            IDENTITY (1, 1) NOT NULL,
    [Name]              NVARCHAR (100) NOT NULL,
    [Description]       NVARCHAR (MAX) NULL,
    [WgerId]            INT            NULL,
    [CreatedBy]         INT            NOT NULL,
    [IsPublic]          BIT            DEFAULT ((0)) NOT NULL,
    PRIMARY KEY CLUSTERED ([WorkoutTemplateId] ASC),
    CONSTRAINT [FK_WorkoutTemplate_User] FOREIGN KEY ([CreatedBy]) REFERENCES [dbo].[Users] ([UserId])
);
GO

CREATE TABLE [dbo].[WorkoutDay] (
    [WorkoutDayId]      INT           IDENTITY (1, 1) NOT NULL,
    [WorkoutTemplateId] INT           NOT NULL,
    [DayOrder]          INT           NOT NULL,
    [Name]              NVARCHAR (50) NULL,
    PRIMARY KEY CLUSTERED ([WorkoutDayId] ASC),
    CONSTRAINT [FK_WorkoutDay_WorkoutTemplate] FOREIGN KEY ([WorkoutTemplateId]) REFERENCES [dbo].[WorkoutTemplate] ([WorkoutTemplateId]) ON DELETE CASCADE
);
GO

CREATE TABLE [dbo].[TemplateExercise] (
    [TemplateExerciseId] INT            IDENTITY (1, 1) NOT NULL,
    [WorkoutDayId]       INT            NOT NULL,
    [ExerciseCatalogId]  INT            NOT NULL,
    [ExerciseOrder]      INT            NOT NULL,
    [Reps]               NVARCHAR (50)  NULL,
    [Sets]               INT            NULL,
    [RestSeconds]        INT            NULL,
    [Notes]              NVARCHAR (MAX) NULL,
    PRIMARY KEY CLUSTERED ([TemplateExerciseId] ASC),
    CONSTRAINT [FK_TemplateExercise_ExerciseCatalog] FOREIGN KEY ([ExerciseCatalogId]) REFERENCES [dbo].[ExerciseCatalog] ([ExerciseCatalogId]),
    CONSTRAINT [FK_TemplateExercise_WorkoutDay] FOREIGN KEY ([WorkoutDayId]) REFERENCES [dbo].[WorkoutDay] ([WorkoutDayId]) ON DELETE CASCADE
);
GO

CREATE TABLE [dbo].[WorkoutLog] (
    [WorkoutLogId]      INT      IDENTITY (1, 1) NOT NULL,
    [UserId]            INT      NOT NULL,
    [WorkoutTemplateId] INT      NULL,
    [DurationMin]       INT      NULL,
    [CaloriesBurned]    INT      NULL,
    [LoggedAt]          DATETIME DEFAULT (getdate()) NULL,
    PRIMARY KEY CLUSTERED ([WorkoutLogId] ASC),
    CONSTRAINT [FK_WorkoutLog_Template] FOREIGN KEY ([WorkoutTemplateId]) REFERENCES [dbo].[WorkoutTemplate] ([WorkoutTemplateId]),
    CONSTRAINT [FK_WorkoutLog_User] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([UserId])
);
GO

CREATE TABLE [dbo].[WorkoutLogExercise] (
    [WorkoutLogExerciseId] INT            IDENTITY (1, 1) NOT NULL,
    [WorkoutLogId]         INT            NOT NULL,
    [ExerciseCatalogId]    INT            NOT NULL,
    [Reps]                 INT            NULL,
    [Sets]                 INT            NULL,
    [WeightKg]             FLOAT (53)     NULL,
    [DurationMin]          INT            NULL,
    [CaloriesBurned]       INT            NULL,
    [PersonalRecord]       BIT            DEFAULT ((0)) NULL,
    [Notes]                NVARCHAR (MAX) NULL,
    PRIMARY KEY CLUSTERED ([WorkoutLogExerciseId] ASC),
    CONSTRAINT [FK_WorkoutLogExercise_ExerciseCatalog] FOREIGN KEY ([ExerciseCatalogId]) REFERENCES [dbo].[ExerciseCatalog] ([ExerciseCatalogId]),
    CONSTRAINT [FK_WorkoutLogExercise_WorkoutLog] FOREIGN KEY ([WorkoutLogId]) REFERENCES [dbo].[WorkoutLog] ([WorkoutLogId]) ON DELETE CASCADE
);
GO

CREATE TABLE [dbo].[Meals] (
    [MealId]      INT            IDENTITY (1, 1) NOT NULL,
    [UserId]      INT            NOT NULL,
    [Name]        NVARCHAR (100) NOT NULL,
    [Description] NVARCHAR (255) NULL,
    [LoggedAt]    DATETIME       DEFAULT (getdate()) NULL,
    [Source]      NVARCHAR (50)  NULL,
    PRIMARY KEY CLUSTERED ([MealId] ASC),
    FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([UserId])
);
GO

CREATE TABLE [dbo].[FoodItems] (
    [FoodItemId] INT            IDENTITY (1, 1) NOT NULL,
    [MealId]     INT            NOT NULL,
    [Name]       NVARCHAR (100) NOT NULL,
    [Quantity]   NVARCHAR (50)  NULL,
    [Calories]   INT            NULL,
    [Source]     NVARCHAR (50)  NULL,
    PRIMARY KEY CLUSTERED ([FoodItemId] ASC),
    FOREIGN KEY ([MealId]) REFERENCES [dbo].[Meals] ([MealId]) ON DELETE CASCADE
);
GO

CREATE TABLE [dbo].[UserGoals] (
    [GoalId]         INT           IDENTITY (1, 1) NOT NULL,
    [UserId]         INT           NOT NULL,
    [GoalType]       NVARCHAR (20) NULL,
    [WeeklyTargetKg] FLOAT (53)    NULL,
    [TargetCalories] INT           NULL,
    [AutoSuggested]  BIT           DEFAULT ((1)) NULL,
    [GoalStartDate]  DATE          NULL,
    PRIMARY KEY CLUSTERED ([GoalId] ASC),
    FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([UserId])
);
GO

CREATE TABLE [dbo].[UserProfiles] (
    [ProfileId]     INT           IDENTITY (1, 1) NOT NULL,
    [UserId]        INT           NOT NULL,
    [Age]           INT           NULL,
    [Gender]        NVARCHAR (10) NULL,
    [HeightCm]      INT           NULL,
    [WeightKg]      FLOAT (53)    NULL,
    [ActivityLevel] NVARCHAR (50) NULL,
    [BMI]           FLOAT (53)    NULL,
    [BMR]           FLOAT (53)    NULL,
    [TDEE]          FLOAT (53)    NULL,
    PRIMARY KEY CLUSTERED ([ProfileId] ASC),
    FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([UserId])
);
GO

CREATE TABLE [dbo].[WeightLogs] (
    [LogId]    INT        IDENTITY (1, 1) NOT NULL,
    [UserId]   INT        NOT NULL,
    [WeightKg] FLOAT (53) NOT NULL,
    [LoggedAt] DATETIME   DEFAULT (getdate()) NULL,
    PRIMARY KEY CLUSTERED ([LogId] ASC),
    FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([UserId])
);
GO

CREATE TABLE [dbo].[ActivityLogs] (
    [ActivityId] INT            IDENTITY (1, 1) NOT NULL,
    [UserId]     INT            NOT NULL,
    [Action]     NVARCHAR (100) NULL,
    [Details]    NVARCHAR (MAX) NULL,
    [CreatedAt]  DATETIME       DEFAULT (getdate()) NULL,
    PRIMARY KEY CLUSTERED ([ActivityId] ASC),
    FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([UserId])
);
GO

ALTER TABLE Meals ADD TotalCalories INT NULL;

SELECT * FROM ACTLOGS



CREATE TABLE WeeklyWorkoutPlans(
    PlanId INT PRIMARY KEY IDENTITY(1,1),
    UserId INT NOT NULL,
    PlanName NVARCHAR(100),
    CreatedDate DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (UserId) REFERENCES Users(UserId)
);

CREATE TABLE DailyWorkouts (
    DailyWorkoutId INT PRIMARY KEY IDENTITY(1,1),
    PlanId INT NOT NULL,
    DayOfWeek NVARCHAR(20),
    ExerciseId INT NOT NULL,
    FOREIGN KEY (PlanId) REFERENCES WeeklyWorkoutPlans(PlanId),
    FOREIGN KEY (ExerciseId) REFERENCES ExerciseCatalog(ExerciseCatalogId)
);