import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { EngagementService } from '../../../core/services/engagement.service';
import { Dashboard, EngagementSummary } from '../../../core/models';

@Component({
  standalone: false,
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss']
})
export class DashboardComponent implements OnInit {
  dashboard: Dashboard | null = null;
  isLoading = true;
  error: string | null = null;

  constructor(
    private engagementService: EngagementService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadDashboard();
  }

  onEngagementClick(engagement: EngagementSummary): void {
    this.router.navigate(['/engagements', engagement.id]);
  }

  onRetry(): void {
    this.loadDashboard();
  }

  private loadDashboard(): void {
    this.isLoading = true;
    this.error = null;
    this.engagementService.getDashboard().subscribe({
      next: data => {
        this.dashboard = data;
        this.isLoading = false;
      },
      error: () => {
        this.error = 'Failed to load dashboard. Please try again.';
        this.isLoading = false;
      }
    });
  }
}
