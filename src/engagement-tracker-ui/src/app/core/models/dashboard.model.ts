import { EngagementSummary } from './engagement.model';

export interface Dashboard {
  totalEngagements: number;
  activeEngagements: number;
  totalHoursLogged: number;
  totalBudgetHours: number;
  overallUtilizationPercent: number;
  engagementsOnTrack: number;
  engagementsAtRisk: number;
  engagementsOverBudget: number;
  topEngagements: EngagementSummary[];
}
