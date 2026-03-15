export interface TimeEntry {
  id: number;
  engagementId: number;
  engagementTitle: string;
  userName: string;
  hours: number;
  date: string;
  description: string;
}

export interface CreateTimeEntry {
  engagementId: number;
  hours: number;
  date: string;
  description: string;
}
