package com.example.ToDoAPP.repository;

import com.example.ToDoAPP.model.Task;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.data.jpa.repository.Query;
import org.springframework.data.repository.query.Param;
import org.springframework.stereotype.Repository;

import java.time.LocalDate;
import java.time.LocalDateTime;
import java.util.List;

@Repository
public interface TaskRepository extends JpaRepository<Task, Long> {

    List<Task> findByDueDate(LocalDate date);

    // Поиск задач по диапазону дат
    List<Task> findByDueDateBetween(LocalDateTime start, LocalDateTime end);

    // Просроченные и невыполненные
    List<Task> findByDueDateBeforeAndCompletedFalse(LocalDateTime now);

    // По статусу
    List<Task> findByCompleted(boolean completed);

}