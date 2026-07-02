import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TaskService } from '../../../../core/services/task.service';
import { TaskItem } from '../../../../core/models/task.model';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-records',
  standalone: true,
  imports: [CommonModule, MatIconModule, MatButtonModule, RouterLink],
  templateUrl: './records.html'
})
export class RecordsComponent implements OnInit {
  tasks: TaskItem[] = [];
  isLoading = true;

  constructor(private taskService: TaskService) {}

  ngOnInit(): void {
    this.loadRecords();
  }

  loadRecords(): void {
    this.isLoading = true;
    this.taskService.getTasks().subscribe({
      next: (data) => {
        this.tasks = data;
        this.isLoading = false;
      },
      error: (err) => {
        console.error('Error cargando registros', err);
        this.isLoading = false;
      }
    });
  }

  onDelete(id: string): void {
    if (confirm('Are you sure you want to delete this record?')) {
      this.taskService.deleteTask(id).subscribe({
        next: () => {
          this.loadRecords(); 
        },
        error: (err) => console.error('Error al eliminar', err)
      });
    }
  }
}