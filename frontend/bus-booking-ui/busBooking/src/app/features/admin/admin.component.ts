import { Component, inject, OnInit, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { AdminService } from '../../core/services/admin.service';
import { NotificationService } from '../../core/services/notification.service';
import { AdminBookingResponse, AdminBusResponse, BusOperatorResponse } from '../../core/models/admin.model';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-admin',
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './admin.component.html',
  styles: ``,
})
export class AdminComponent implements OnInit {
  fb = inject(FormBuilder);
  adminService = inject(AdminService);
  notify = inject(NotificationService);

  buses= signal<AdminBusResponse[]>([]);
  bookings= signal<AdminBookingResponse[]>([]);
  operators = signal<BusOperatorResponse[]>([]);

  selectedBus?: AdminBusResponse;
  loadingBuses = signal<boolean>(true);
  loadingBookings = signal<boolean>(true);
  loadingOperators = signal<boolean>(true);

  savingBus = signal<boolean>(false);

  busForm = this.fb.group({
    busOperatorId: [null as number | null, Validators.required],
    busNumber: ["", Validators.required],
    busType: ["", Validators.required],
    source: ["", Validators.required],
    destination: ["", Validators.required],
    departureTime: ["09:00", Validators.required],
    durationMinutes: [180, [Validators.required, Validators.min(1), Validators.max(2880)]],
    totalSeats: [40, [Validators.required, Validators.min(1), Validators.max(100)]],
    farePerSeat: [500, [Validators.required, Validators.min(0.01)]]
  });

  ngOnInit(): void {
    this.loadBuses();
    this.loadOperators();
  }

  loadBuses(): void{
    this.loadingBuses.set(true);
    this.adminService.getBuses().subscribe({
      next: (res) => {
        this.buses.set(res);
        this.loadingBuses.set(false);
      },
      error: () =>{
        this.loadingBuses.set(false);
      }
    });
  }

  loadOperators(): void{
    this.loadingOperators.set(true);
    this.adminService.getBusOperators().subscribe({
      next: (res) => {
        this.operators.set(res);
        this.loadingOperators.set(false);
      },
      error: () =>{
        this.loadingOperators.set(false);
      }
    });
  }

  viewBookings(bus: AdminBusResponse): void{
    this.selectedBus = bus;
    this.loadingBookings.set(true);
    this.adminService.getBookingsForBus(bus.busId).subscribe({
      next: (res) =>{
        this.bookings.set(res);
        this.loadingBookings.set(false);
      },
      error: () =>{
        this.loadingBookings.set(false);
      }
    })
  }

  submitBus(): void{
    if(this.busForm.invalid){
      this.busForm.markAllAsTouched();
      return;
    }

    const value = this.busForm.getRawValue();
    this.savingBus.set(true);
    this.adminService.createBus({
      busOperatorId: value.busOperatorId!,
      busNumber: value.busNumber!,
      busType: value.busType!,
      source: value.source!,
      destination: value.destination!,
      departureTime: value.departureTime!,
      durationMinutes: value.durationMinutes!,
      totalSeats: value.totalSeats!,
      farePerSeat: value.farePerSeat!
    }).subscribe({
      next: () =>{
        this.notify.success("Bus created successfully!!");
        this.savingBus.set(false);
        this.busForm.reset({
          busOperatorId: null,
          busNumber: "",
          busType: "",
          source: "",
          destination: "",
          departureTime: "09:00",
          durationMinutes: 180,
          totalSeats: 40,
          farePerSeat: 500
        });
        this.loadBuses();
        this.loadOperators();
      },
      error: (err) =>{
        this.savingBus.set(false);
      }
    });
  }


}
