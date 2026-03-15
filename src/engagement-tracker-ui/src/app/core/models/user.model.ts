export type UserRole = 'Associate' | 'Manager' | 'Partner';

export interface UserProfile {
  id: number;
  email: string;
  firstName: string;
  lastName: string;
  role: UserRole;
}
