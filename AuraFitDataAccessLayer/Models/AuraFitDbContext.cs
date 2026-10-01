using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace AuraFitDataAccessLayer.Models;

public partial class AuraFitDbContext : DbContext
{
    public AuraFitDbContext()
    {
    }

    public AuraFitDbContext(DbContextOptions<AuraFitDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ActivityLog> ActivityLogs { get; set; }

    public virtual DbSet<ExerciseCatalog> ExerciseCatalog { get; set; }

    public virtual DbSet<FoodItem> FoodItems { get; set; }

    public virtual DbSet<Meal> Meals { get; set; }

    public virtual DbSet<TemplateExercise> TemplateExercises { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserGoal> UserGoals { get; set; }

    public virtual DbSet<UserProfile> UserProfiles { get; set; }

    public virtual DbSet<WeightLog> WeightLogs { get; set; }

    public virtual DbSet<WorkoutDay> WorkoutDays { get; set; }

    public virtual DbSet<WorkoutLog> WorkoutLogs { get; set; }

    public virtual DbSet<WorkoutLogExercise> WorkoutLogExercises { get; set; }

    public virtual DbSet<WorkoutTemplate> WorkoutTemplates { get; set; }
    public DbSet<WeeklyWorkoutPlan> WeeklyWorkoutPlans { get; set; }
    public DbSet<DailyWorkout> DailyWorkouts { get; set; }

    //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //{
    //    var builder = new ConfigurationBuilder()
    //                       .SetBasePath(Directory.GetCurrentDirectory())
    //                       .AddJsonFile("appsettings.json");
    //    var config = builder.Build();
    //    var connectionString = config.GetConnectionString("AuraFitDBConnection");
    //    if (!optionsBuilder.IsConfigured)
    //    {
    //        optionsBuilder.UseSqlServer(connectionString);
    //    }
    //}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ActivityLog>(entity =>
        {
            entity.HasKey(e => e.ActivityId).HasName("PK__Activity__45F4A7914376A151");

            entity.Property(e => e.Action).HasMaxLength(100);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.User).WithMany(p => p.ActivityLogs)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ActivityL__UserI__412EB0B6");
        });

        modelBuilder.Entity<ExerciseCatalog>(entity =>
        {
            entity.HasKey(e => e.ExerciseCatalogId).HasName("PK__Exercise__03434DD074578212");

            entity.ToTable("ExerciseCatalog");

            entity.Property(e => e.Category).HasMaxLength(50);
            entity.Property(e => e.Equipment).HasMaxLength(255);
            entity.Property(e => e.ImageUrl).HasMaxLength(255);
            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<FoodItem>(entity =>
        {
            entity.HasKey(e => e.FoodItemId).HasName("PK__FoodItem__464DC8121DDDCCE6");

            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Quantity).HasMaxLength(50);
            entity.Property(e => e.Source).HasMaxLength(50);

            entity.HasOne(d => d.Meal).WithMany(p => p.FoodItems)
                .HasForeignKey(d => d.MealId)
                .HasConstraintName("FK__FoodItems__MealI__398D8EEE");
        });

        modelBuilder.Entity<Meal>(entity =>
        {
            entity.HasKey(e => e.MealId).HasName("PK__Meals__ACF6A63D6D731F10");

            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.LoggedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Source).HasMaxLength(50);

            entity.HasOne(d => d.User).WithMany(p => p.Meals)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Meals__UserId__36B12243");
        });

        modelBuilder.Entity<TemplateExercise>(entity =>
        {
            entity.HasKey(e => e.TemplateExerciseId).HasName("PK__Template__5A70BC40522D30AF");

            entity.ToTable("TemplateExercise");

            entity.Property(e => e.Reps).HasMaxLength(50);

            entity.HasOne(d => d.ExerciseCatalog).WithMany(p => p.TemplateExercises)
                .HasForeignKey(d => d.ExerciseCatalogId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TemplateExercise_ExerciseCatalog");

            entity.HasOne(d => d.WorkoutDay).WithMany(p => p.TemplateExercises)
                .HasForeignKey(d => d.WorkoutDayId)
                .HasConstraintName("FK_TemplateExercise_WorkoutDay");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CC4C9B9412BA");

            entity.HasIndex(e => e.Username, "UQ__Users__536C85E43EA5F5C2").IsUnique();

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.PasswordHash).HasMaxLength(256);
            entity.Property(e => e.Username).HasMaxLength(50);
        });

        modelBuilder.Entity<UserGoal>(entity =>
        {
            entity.HasKey(e => e.GoalId).HasName("PK__UserGoal__8A4FFFD1FBDE3909");

            entity.Property(e => e.AutoSuggested).HasDefaultValue(true);
            entity.Property(e => e.GoalType).HasMaxLength(20);

            entity.HasOne(d => d.User).WithMany(p => p.UserGoals)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__UserGoals__UserI__2C3393D0");
        });

        modelBuilder.Entity<UserProfile>(entity =>
        {
            entity.HasKey(e => e.ProfileId).HasName("PK__UserProf__290C88E4BFCBCDC7");

            entity.Property(e => e.ActivityLevel).HasMaxLength(50);
            entity.Property(e => e.Bmi).HasColumnName("BMI");
            entity.Property(e => e.Bmr).HasColumnName("BMR");
            entity.Property(e => e.Gender).HasMaxLength(10);
            entity.Property(e => e.Tdee).HasColumnName("TDEE");

            entity.HasOne(d => d.User).WithMany(p => p.UserProfiles)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__UserProfi__UserI__286302EC");
        });

        modelBuilder.Entity<WeightLog>(entity =>
        {
            entity.HasKey(e => e.LogId).HasName("PK__WeightLo__5E54864883F6F883");

            entity.Property(e => e.LoggedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.User).WithMany(p => p.WeightLogs)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__WeightLog__UserI__3D5E1FD2");
        });

        modelBuilder.Entity<WorkoutDay>(entity =>
        {
            entity.HasKey(e => e.WorkoutDayId).HasName("PK__WorkoutD__D459FCA99C349B6C");

            entity.ToTable("WorkoutDay");

            entity.Property(e => e.Name).HasMaxLength(50);

            entity.HasOne(d => d.WorkoutTemplate).WithMany(p => p.WorkoutDays)
                .HasForeignKey(d => d.WorkoutTemplateId)
                .HasConstraintName("FK_WorkoutDay_WorkoutTemplate");
        });

        modelBuilder.Entity<WorkoutLog>(entity =>
        {
            entity.HasKey(e => e.WorkoutLogId).HasName("PK__WorkoutL__59259275526CA055");

            entity.ToTable("WorkoutLog");

            entity.Property(e => e.LoggedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.User).WithMany(p => p.WorkoutLogs)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_WorkoutLog_User");

            entity.HasOne(d => d.WorkoutTemplate).WithMany(p => p.WorkoutLogs)
                .HasForeignKey(d => d.WorkoutTemplateId)
                .HasConstraintName("FK_WorkoutLog_Template");
        });

        modelBuilder.Entity<WorkoutLogExercise>(entity =>
        {
            entity.HasKey(e => e.WorkoutLogExerciseId).HasName("PK__WorkoutL__1AB8B4B2549F8FD9");

            entity.ToTable("WorkoutLogExercise");

            entity.Property(e => e.PersonalRecord).HasDefaultValue(false);
            entity.Property(e => e.Reps).HasMaxLength(50);

            entity.HasOne(d => d.ExerciseCatalog).WithMany(p => p.WorkoutLogExercises)
                .HasForeignKey(d => d.ExerciseCatalogId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_WorkoutLogExercise_ExerciseCatalog");

            entity.HasOne(d => d.WorkoutLog).WithMany(p => p.WorkoutLogExercises)
                .HasForeignKey(d => d.WorkoutLogId)
                .HasConstraintName("FK_WorkoutLogExercise_WorkoutLog");
        });

        modelBuilder.Entity<WorkoutTemplate>(entity =>
        {
            entity.HasKey(e => e.WorkoutTemplateId).HasName("PK__WorkoutT__8959FF4F9B95C549");

            entity.ToTable("WorkoutTemplate");

            entity.Property(e => e.Name).HasMaxLength(100);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.WorkoutTemplates)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_WorkoutTemplate_User");
        });
        modelBuilder.Entity<DailyWorkout>()
            .HasOne(dw => dw.WeeklyWorkoutPlan)
            .WithMany(wp => wp.DailyWorkouts)
            .HasForeignKey(dw => dw.PlanId);

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
