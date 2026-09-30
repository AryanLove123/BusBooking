export interface ApiErrorBody {
  statusCode: number;
  message: string;
  errors?: Record<string, string[]>;
  traceId?: string;
}
