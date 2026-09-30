import { Component, inject, OnInit, signal } from '@angular/core';
import { BookingResponse } from '../../../core/models/booking.model';
import { BookingService } from '../../../core/services/booking.service';
import { Router } from '@angular/router';
import { NotificationService } from '../../../core/services/notification.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-view-bookings',
  imports: [CommonModule],
  templateUrl: './view-bookings.component.html',
  styles: ``,
})
export class ViewBookingsComponent implements OnInit {
  bookingService = inject(BookingService);
  notify = inject(NotificationService);
  router = inject(Router);
  bookings = signal<BookingResponse[]>([]);
  loading = signal<boolean>(true);
  cancellingId: number | null = null;

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.loading.set(true);
    this.bookingService.getMine().subscribe({
      next: (res) => {
        this.bookings.set(res);
        this.loading.set(false);
      },
      error: () => {
        this.loading.set(false);
      },
    });
  }

  edit(booking: BookingResponse): void {
    this.router.navigate(['/bookings', booking.bookingId, 'edit']);
  }

  cancel(booking: BookingResponse): void {
    if (!confirm(`Cancel booking #${booking.bookingId}? This cannot be undone.`)) return;

    this.cancellingId = booking.bookingId;
    this.bookingService.cancel(this.cancellingId).subscribe({
      next: () => {
        this.notify.success('Booking cancelled and seats released.');
        this.cancellingId = null;
        this.load();
      },
      error: () => {
        this.cancellingId = null;
      },
    });
  }

  hasDeparted(booking: BookingResponse): boolean {
    return new Date(booking.departureUtc).getTime() <= Date.now();
  }
}
