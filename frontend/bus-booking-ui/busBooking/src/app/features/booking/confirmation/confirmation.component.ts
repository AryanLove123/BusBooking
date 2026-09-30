import { Component, inject, OnInit, signal } from '@angular/core';
import { BookingResponse } from '../../../core/models/booking.model';
import { BookingService } from '../../../core/services/booking.service';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-confirmation',
  imports: [CommonModule, RouterLink],
  templateUrl: './confirmation.component.html',
  styles: ``,
})
export class ConfirmationComponent implements OnInit {
  bookingService = inject(BookingService);
  route = inject(ActivatedRoute);
  booking = signal<BookingResponse | undefined>(undefined);
  loading = signal<boolean>(true);
  
  ngOnInit(): void {
    const id = Number(this.route.snapshot.paramMap.get("id"));
    this.bookingService.getById(id).subscribe({
      next: (res) => {
        this.booking.set(res);
        this.loading.set(false);
      },
      error: () =>{
        this.loading.set(false);
      }
    })
  }
}
