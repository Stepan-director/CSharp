package com.example.ToDoAPP.controller;

import com.example.ToDoAPP.model.Task;
import com.example.ToDoAPP.service.TaskService;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.format.annotation.DateTimeFormat;
import org.springframework.web.bind.annotation.*;

import java.time.LocalDateTime;
import java.util.List;

@RestController
@RequestMapping("/api/tasks")
@CrossOrigin(origins = "http://localhost:5000")
public class TaskController {

    @Autowired
    private TaskService taskService;

    // ПОСМОТРЕТЬ ВСЕ
    @GetMapping
    public List<Task> getAllTasks() {
        return taskService.getAllTasks();
    }

    // ДОБАВИТЬ
    @PostMapping
    public Task addTask(@RequestBody Task task) {
        return taskService.addTask(task);
    }

    // ИЗМЕНИТЬ
    @PutMapping("/{id}")
    public Task updateTask(@PathVariable Long id, @RequestBody Task task) {
        return taskService.updateTask(id, task);
    }

    // УДАЛИТЬ
    @DeleteMapping("/{id}")
    public void deleteTask(@PathVariable Long id) {
        taskService.deleteTask(id);
    }

    // ПОЛУЧИТЬ ЗАДАЧИ ПО ДАТЕ
    @GetMapping("/by-date")
    public List<Task> getTasksByDate(@RequestParam @DateTimeFormat(iso = DateTimeFormat.ISO.DATE_TIME) LocalDateTime date) {
        return taskService.getTasksByDate(date);
    }

    // ПОЛУЧИТЬ ПРОСРОЧЕННЫЕ ЗАДАЧИ
    @GetMapping("/overdue")
    public List<Task> getOverdueTasks() {
        return taskService.getOverdueTasks();
    }
}