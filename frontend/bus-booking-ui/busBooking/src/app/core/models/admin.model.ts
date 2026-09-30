export interface AdminBusResponse {
  busId: number;
  busNumber: string;
  busType: string;
  source: string;
  destination: string;
  departureUtc: string;
  arrivalUtc: string;
  totalSeats: number;
  availableSeats: number;
  farePerSeat: number;
}

export interface AdminBookingResponse {
  bookingId: number;
  userId: number;
  userName: string;
  userEmail: string;
  seatsBooked: number;
  totalFare: number;
  status: "Confirmed" | "Cancelled";
  bookingDateUtc: string;
}

export interface CreateBusRequest {
  busOperatorId: number;
  busNumber: string;
  busType: string;
  source: string;
  destination: string;
  departureTime: string;
  durationMinutes: number;
  totalSeats: number;
  farePerSeat: number;
}

export interface BusOperatorResponse {
  operatorId: number;
  name: string;
  contactEmail?: string;
  contactPhone?: string;
  busCount: number;
}