import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';

import { StatusBadgeComponent } from './components/status-badge/status-badge.component';
import { BudgetProgressBarComponent } from './components/budget-progress-bar/budget-progress-bar.component';
import { LoadingSkeletonComponent } from './components/loading-skeleton/loading-skeleton.component';
import { EmptyStateComponent } from './components/empty-state/empty-state.component';
import { ErrorStateComponent } from './components/error-state/error-state.component';
import { HoursPipe } from './pipes/hours.pipe';

const COMPONENTS = [
  StatusBadgeComponent,
  BudgetProgressBarComponent,
  LoadingSkeletonComponent,
  EmptyStateComponent,
  ErrorStateComponent,
  HoursPipe
];

@NgModule({
  declarations: COMPONENTS,
  imports: [CommonModule, MatIconModule, MatButtonModule],
  exports: COMPONENTS
})
export class SharedModule {}
