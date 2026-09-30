import { Component, inject, OnInit, signal } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { BookingService } from '../../../core/services/booking.service';
import { ActivatedRoute, Router } from '@angular/router';
import { NotificationService } from '../../../core/services/notification.service';
import { BusSearchResult } from '../../../core/models/bus.model';
import { BookingResponse } from '../../../core/models/booking.model';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-book',
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './book.component.html',
  styles: ``,
})
export class BookComponent implements OnInit {
  fb = inject(FormBuilder);
  bookingService = inject(BookingService);
  router = inject(Router);
  route = inject(ActivatedRoute);
  notify = inject(NotificationService);

  isEditMode = signal(false);
  loading = signal(true);
  submitting = signal(false);
  busIdForCreate = signal(0);

  bus = signal<BusSearchResult | undefined>(undefined);
  existingBooking = signal<BookingResponse | undefined>(undefined);

  form: FormGroup = this.fb.group({
    seatsRequested: [1, [Validators.required, Validators.min(1), Validators.max(10)]],
    passengers: this.fb.array([]),
  });

  get passengers(): FormArray {
    return this.form.get('passengers') as FormArray;
  }

  setPassengerCount(count: number): void {
    while (this.passengers.length < count) {
      this.passengers.push(
        this.fb.group({
          name: ['', Validators.required],
          age: [18, [Validators.required, Validators.min(1), Validators.max(120)]],
          gender: [0, Validators.required],
        }),
      );
    }

    while (this.passengers.length > count) {
      this.passengers.removeAt(this.passengers.length - 1);
    }
  }

  ngOnInit(): void {
    const editMode = this.route.snapshot.routeConfig?.path === 'bookings/:id/edit';
    this.isEditMode.set(editMode);

    if (editMode) {
      this.loadForEdit();
    } else {
      this.loadForCreate();
    }
  }

  loadForEdit(): void {
    const bookingId = Number(this.route.snapshot.paramMap.get('id'));

    this.bookingService.getById(bookingId).subscribe({
      next: (booking) => {
        this.existingBooking.set(booking);

        this.form.patchValue({
          seatsRequested: booking.seatsBooked,
        });

        this.setPassengerCount(booking.seatsBooked);

        booking.passengers.forEach((passenger, index) => {
          this.passengers.at(index).patchValue(passenger);
        });

        this.loading.set(false);
      },

      error: (err) => {
        console.error('Failed to load booking:', err);

        this.loading.set(false);
      },
    });
  }

  loadForCreate(): void {
    const busId = Number(this.route.snapshot.paramMap.get('busId'));

    this.busIdForCreate.set(busId);

    const navState = this.router.getCurrentNavigation()?.extras.state ?? history.state;

    const bus = navState?.['bus'] as BusSearchResult | undefined;

    this.bus.set(bus);

    const maxSeats = bus ? Math.min(bus.availableSeats, 10) : 10;

    const seatsControl = this.form.get('seatsRequested');

    seatsControl?.setValidators([Validators.required, Validators.min(1), Validators.max(maxSeats)]);

    seatsControl?.updateValueAndValidity();

    this.setPassengerCount(1);

    this.loading.set(false);
  }

  onSeatsChanged(): void {
    const seats = Number(this.form.value.seatsRequested) || 1;

    this.setPassengerCount(seats);
  }

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const value = this.form.getRawValue();

    this.submitting.set(true);

    if (this.isEditMode() && this.existingBooking()) {
      const bookingId = this.existingBooking()!.bookingId;

      this.bookingService
        .update(bookingId, {
          seatsRequested: value.seatsRequested,
          passengers: value.passengers,
        })
        .subscribe({
          next: () => {
            this.notify.success('Booking updated successfully!');

            this.router.navigate(['/view-bookings']);
          },

          error: (err) => {
            console.error('Failed to update booking:', err);

            this.submitting.set(false);
          },
        });
    } else {
      this.bookingService
        .create({
          busId: this.busIdForCreate(),

          seatsRequested: value.seatsRequested,

          passengers: value.passengers,
        })
        .subscribe({
          next: (res) => {
            this.router.navigate(['/booking-confirmation', res.bookingId]);
          },

          error: (err) => {
            console.error('Failed to create booking:', err);

            this.submitting.set(false);
          },
        });
    }
  }
}
