export interface BusSearchResult{
    busId: number;
    operatorName: string;
    busNumber: string;
    busType: string;
    source: string;
    destination: string;
    departureUtc: string;
    arrivalUtc: string;
    availableSeats: number;
    farePerSeat: number;
}

export interface BusSearchParams {
  source: string;
  destination: string;
  departureAfter?: string;
  arrivalBefore?: string;
}
