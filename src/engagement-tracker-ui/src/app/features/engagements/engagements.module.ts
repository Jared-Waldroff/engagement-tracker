import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';
import { MatTableModule } from '@angular/material/table';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatDialogModule } from '@angular/material/dialog';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatSnackBarModule } from '@angular/material/snack-bar';

import { SharedModule } from '../../shared/shared.module';
import { EngagementsRoutingModule } from './engagements-routing.module';
import { EngagementListComponent } from './engagement-list/engagement-list.component';
import { EngagementDetailComponent } from './engagement-detail/engagement-detail.component';
import { EngagementFormDialogComponent } from './engagement-form-dialog/engagement-form-dialog.component';

@NgModule({
  declarations: [
    EngagementListComponent,
    EngagementDetailComponent,
    EngagementFormDialogComponent
  ],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    EngagementsRoutingModule,
    SharedModule,
    MatTableModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatButtonModule,
    MatIconModule,
    MatDialogModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatSnackBarModule
  ]
})
export class EngagementsModule {}
