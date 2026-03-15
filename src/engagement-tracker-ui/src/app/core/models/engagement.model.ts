export type EngagementStatus = 'Planning' | 'Active' | 'OnHold' | 'Completed' | 'Cancelled';
export type BudgetStatus = 'OnTrack' | 'AtRisk' | 'OverBudget';

export interface EngagementSummary {
  id: number;
  clientName: string;
  title: string;
  status: EngagementStatus;
  budgetHours: number;
  hoursLogged: number;
  budgetUtilizationPercent: number;
  budgetStatus: BudgetStatus;
  managerName: string;
  startDate: string;
  endDate: string | null;
}

export interface EngagementDetail {
  id: number;
  clientName: string;
  clientIndustry: string;
  title: string;
  description: string;
  status: EngagementStatus;
  budgetHours: number;
  hourlyRate: number;
  hoursLogged: number;
  budgetUtilizationPercent: number;
  budgetStatus: BudgetStatus;
  totalBudgetDollars: number;
  spentDollars: number;
  managerName: string;
  startDate: string;
  endDate: string | null;
  recentTimeEntries: TimeEntrySummary[];
  hoursByUser: UserHoursSummary[];
}

export interface TimeEntrySummary {
  id: number;
  engagementId: number;
  engagementTitle: string;
  userName: string;
  hours: number;
  date: string;
  description: string;
}

export interface UserHoursSummary {
  userId: number;
  userName: string;
  role: string;
  totalHours: number;
}

export interface EngagementFilters {
  status?: EngagementStatus | '';
  search?: string;
}

export interface CreateEngagement {
  title: string;
  description: string;
  clientId: number;
  budgetHours: number;
  hourlyRate: number;
  startDate: string;
  endDate?: string;
}

export interface UpdateEngagement {
  title?: string;
  description?: string;
  status?: EngagementStatus;
  budgetHours?: number;
  hourlyRate?: number;
  endDate?: string;
}
