import { Injectable, signal } from '@angular/core';

export interface Notification {
  id: number;
  message: string;
  type: "success" | "error";
}

@Injectable({
  providedIn: 'root',
})
export class NotificationService {
  nextId = 1;
  notifications = signal<Notification[]>([]);
  
  success(message: string): void{
    this.push(message, "success");
  }

  error(message: string): void{
    this.push(message, "error");
  }

  dismiss(id: number): void{
    this.notifications.update((list) => list.filter((n) => n.id!==id)); 
  }

  push(message: string, type: "success" | "error"): void{
    const id = this.nextId++;
    this.notifications.update((list) => [...list, {id, message, type}]);
    setTimeout(() => {
      this.dismiss(id);
    }, 3000);
  }
}
