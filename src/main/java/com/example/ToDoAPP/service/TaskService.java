package com.example.ToDoAPP.service;


import com.example.ToDoAPP.model.Task;
import com.example.ToDoAPP.repository.TaskRepository;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Service;

import java.time.LocalDateTime;
import java.util.List;

@Service
public class TaskService {

    @Autowired
    private TaskRepository taskRepository;

    // ПОСМОТРЕТЬ ВСЕ
    public List<Task> getAllTasks() {
        return taskRepository.findAll();
    }

    // ДОБАВИТЬ
    public Task addTask(Task task) {
        task.setId(null);

        if (task.getDueDate() == null) {

            task.setDueDate(LocalDateTime.now().plusDays(1));

        }
        return taskRepository.save(task);
    }

    // ИЗМЕНИТЬ
    public Task updateTask(Long id, Task newTask) {
        Task task = taskRepository.findById(id).orElseThrow();

        task.setTitle(newTask.getTitle());

        task.setCompleted(newTask.isCompleted());

        if (newTask.getDueDate() != null) {

            task.setDueDate(newTask.getDueDate());

        }
        return taskRepository.save(task);
    }

    // УДАЛИТЬ
    public void deleteTask(Long id) {
        taskRepository.deleteById(id);
    }

    // чекнуть по дате
    public List<Task> getTasksByDate(LocalDateTime date) {
        return taskRepository.findByDueDateBetween(
                date.withHour(0).withMinute(0).withSecond(0),
                date.withHour(23).withMinute(59).withSecond(59)
        );
    }

    // чекнуть просроченные
    public List<Task> getOverdueTasks() {
        return taskRepository.findByDueDateBeforeAndCompletedFalse(LocalDateTime.now());
    }


}