export type Gender = "Male" | "Female" | "Other";

export interface PassengerDto {
  name: string;
  age: number;
  gender: Gender;
}

export interface CreateBookingRequest {
  busId: number;
  seatsRequested: number;
  passengers: PassengerDto[];
}

export interface UpdateBookingRequest {
  seatsRequested: number;
  passengers: PassengerDto[];
}

export interface BookingResponse {
  bookingId: number;
  busId: number;
  operatorName: string;
  busNumber: string;
  source: string;
  destination: string;
  departureUtc: string;
  arrivalUtc: string;
  seatsBooked: number;
  totalFare: number;
  status: "Confirmed" | "Cancelled";
  bookingDateUtc: string;
  passengers: PassengerDto[];
}
