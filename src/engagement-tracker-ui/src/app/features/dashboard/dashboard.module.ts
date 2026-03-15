import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatIconModule } from '@angular/material/icon';
import { MatTableModule } from '@angular/material/table';

import { SharedModule } from '../../shared/shared.module';
import { DashboardRoutingModule } from './dashboard-routing.module';
import { DashboardComponent } from './dashboard/dashboard.component';
import { StatCardComponent } from './widgets/stat-card/stat-card.component';
import { BudgetDonutComponent } from './widgets/budget-donut/budget-donut.component';
import { TopEngagementsTableComponent } from './widgets/top-engagements-table/top-engagements-table.component';

@NgModule({
  declarations: [
    DashboardComponent,
    StatCardComponent,
    BudgetDonutComponent,
    TopEngagementsTableComponent
  ],
  imports: [
    CommonModule,
    DashboardRoutingModule,
    SharedModule,
    MatIconModule,
    MatTableModule
  ]
})
export class DashboardModule {}
