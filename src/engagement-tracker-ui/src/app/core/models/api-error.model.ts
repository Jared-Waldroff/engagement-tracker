export interface ApiError {
  error: {
    code: string;
    message: string;
    timestamp: string;
    traceId: string;
  };
}
