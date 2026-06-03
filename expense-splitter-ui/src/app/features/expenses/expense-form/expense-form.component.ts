import { GroupMember } from './../../../core/models/group.model';
import { GroupService } from './../../../core/services/group.service';
import { Component, OnInit } from "@angular/core";
import { FormBuilder, FormGroup, Validators } from "@angular/forms";
import { ActivatedRoute } from "@angular/router";
import { Router } from "@angular/router";
import { ExpenseService } from "../../../core/services/expense.service";
import { AuthService } from "../../../core/services/auth.service";
import { MatButton, MatIconButton } from '@angular/material/button';
import { MatIconModule } from "@angular/material/icon";
import { MatCard, MatCardContent } from "@angular/material/card";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { MatFormField, MatLabel, MatError, MatSuffix } from "@angular/material/form-field";
import { MatInput } from "@angular/material/input";
import { MatSelect, MatOption } from "@angular/material/select";
import { MatDatepickerModule } from '@angular/material/datepicker';
import { CommonModule } from "@angular/common";
import { MatProgressSpinner } from "@angular/material/progress-spinner";
import { Group } from "../../../core/models/group.model";
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatDividerModule } from '@angular/material/divider';

@Component({
    selector: 'app-expense-form',
    templateUrl: 'expense-form.component.html',
    styleUrl: 'expense-form.component.scss',
    standalone: true,
    imports: [MatButton, MatIconButton, MatIconModule, MatCard, MatCardContent, FormsModule, ReactiveFormsModule, MatFormField, MatLabel, MatInput, MatError, MatSelect, MatOption, MatSuffix, MatDatepickerModule, CommonModule, MatProgressSpinner, MatCheckboxModule, MatDividerModule]
})
export class ExpenseFormComponent implements OnInit {
    expenseForm: FormGroup;
    groupId: string | null = null;
    expenseId: string | null = null;
    isEditMode = false;
    isLoading = false;
    errorMessage = '';
    groups: Group[] = [];
    isGroupsLoading = false;
    groupMembers: GroupMember[] = [];
    selectedMemberIds: string[] = [];
    exactAmounts: Record<string, number> = {};
    percentages: Record<string, number> = {};
    isLoadingMembers = false;

    categories = [
        { value: 1, label: 'Food' },
        { value: 2, label: 'Travel' },
        { value: 3, label: 'Rent' },
        { value: 4, label: 'Utilities' },
        { value: 5, label: 'Entertainment' },
        { value: 6, label: 'Shopping' },
        { value: 7, label: 'Medical' },
        { value: 8, label: 'Other' }
    ];

    splitType = [
        { value: 1, label: 'Equal' },
        { value: 2, label: 'Percentage' },
        { value: 3, label: 'Exact' }
    ];

    constructor(private fb: FormBuilder,
        private route: ActivatedRoute,
        private router: Router,
        private expenseService: ExpenseService,
        private authService: AuthService,
        private groupService: GroupService
    ) {
        this.expenseForm = this.fb.group({
            title: ['', [Validators.required, Validators.minLength(2)]],
            amount: ['', [Validators.required, Validators.min(1)]],
            category: [1, Validators.required],
            splitType: [1, Validators.required],
            date: [new Date(), Validators.required],
            groupId: ['', Validators.required]
        })
    }

    ngOnInit(): void {
        this.groupId = this.route.snapshot.queryParamMap.get('groupId');
        if (this.groupId) {
            this.expenseForm.patchValue({ groupId: this.groupId });
            this.loadGroupMembers(this.groupId);
        }

        this.loadGroups();

        this.expenseId = this.route.snapshot.paramMap.get('id');
        this.isEditMode = !!this.expenseId;

        if(this.isEditMode && this.expenseId)
        {
            this.loadExpense(this.expenseId);
        }

        this.expenseForm.get('splitType')?.valueChanges.subscribe(() => {
            this.recalculateSplits();
        });

        this.expenseForm.get('amount')?.valueChanges.subscribe(() => {
            this.recalculateSplits();
        })
    }

    loadGroupMembers(groupId: string) {
        this.isLoadingMembers = true;

        this.groupService.getGroupById(groupId).subscribe({
            next: (group) => {
                this.groupMembers = group.members;

                this.selectedMemberIds = group.members.map((m: GroupMember) => m.userId);

                this.initializeSplits();
                this.isLoadingMembers = false;
            },
            error: () =>{
                this.isLoadingMembers = false;
            }
        });
    }

    initializeSplits(): void {
        const count = this.groupMembers.length;

        this.groupMembers.forEach((m: GroupMember) => {
            this.exactAmounts[m.userId] = 0;
            this.percentages[m.userId] = count > 0 ? Math.round(100 / count) : 0;
        });

        this.recalculateSplits();
    }

    recalculateSplits(): void{
        const splitType = this.expenseForm.get('splitType')?.value;
        const amount = this.expenseForm.get('amount')?.value;
        const count = this.selectedMemberIds.length;

        if(splitType === 1 && count > 0)
        {
            const share = Math.round((amount / count) * 100) / 100;
            this.selectedMemberIds.forEach(id => {
                this.exactAmounts[id] = share
            });
        }
    }

    loadExpense(expenseId: string): void {
        this.isLoading = true;
        this.expenseService.getExpenseById(expenseId).subscribe ({
            next: (expense) => {
                this.expenseForm.patchValue({
                    title: expense.title,
                    amount: expense.amount,
                    category: expense.category,
                    splitType: expense.splitType,
                    date: expense.date,
                    groupId: expense.groupId
                });
                this.isLoading = false;
            },
            error: () => {
                this.isLoading = false;
            }
        })
    }

    loadGroups(): void {
        const userId = this.authService.getCurrentUser()?.id;
        if(!userId)
            return

        this.isGroupsLoading = true;
        this.groupService.getGroupsByUserId(userId).subscribe({
            next: (groups) => {
                this.groups= groups;
                this.isGroupsLoading = false;
            },
            error: () => {
                this.isGroupsLoading = false;
            }
        });
    }

    onSubmit(): void {
        if(this.expenseForm.invalid)
        {
            return;
        }

        if(this.selectedMemberIds.length === 0)
        {
            this.errorMessage = 'Select at least one member to split with.';
            return;
        }

        const splitType = this.expenseForm.get('splitType')?.value;
        const amount = this.expenseForm.get('amount')?.value;

        if(splitType === 2) {
            const total = this.getTotalPercentages();
            if(total != 100) {
                this.errorMessage = `Percentages must total 100%. Current ${total}`;
                return;
            }
        }

        if(splitType === 3){
            const total = this.getTotalExact();
            if(total != amount){
                this.errorMessage = `Split amount (₹${total}) must equal total (₹${amount})`;
                return;
            }
        }

        this.isLoading = true;
        this.errorMessage = '';
        
        const currentUser = this.authService.getCurrentUser();
        const formValue = this.expenseForm.value;

        if(this.isEditMode && this.expenseId)
        {
            this.expenseService.updateExpense(this.expenseId, formValue).subscribe({
                next: () => {
                    this.isLoading = false;
                    this.router.navigate(['/expenses']);
                },
                error: () => {
                    this.isLoading = false;
                    this.errorMessage = "Failed to update expense.";
                }
            })
        }
        else{
            this.expenseService.createExpense({
            ...formValue, paidByUserId: currentUser!.id, 
            splits: this.buildSplits()}).subscribe({
                next: () => {
                    this.isLoading = false;
                    if (this.groupId) {
                        this.router.navigate(['/groups', this.groupId]);
                    } else {
                        this.router.navigate(['/expenses']);
                    }
                },
                error: () => {
                    this.isLoading = false;
                    this.errorMessage = 'Failed to create expense.';
                }
            });
        }
    }

    goBack(): void {
        if(this.groupId){
            this.router.navigate(['/groups',this.groupId]);
        }
        else{
            this.router.navigate(['/expenses']);
        }
    }

    onGroupChange(): void {
        const groupId = this.expenseForm.get('groupId')?.value;

        if(!groupId)
            return;

        this.groupService.getGroupById(groupId).subscribe({
            next: (group) => {
                this.groupMembers = group.members;
                this.selectedMemberIds = group.members.map(m => m.userId);
            }
        })
    }

    buildSplits(): any[] {
        const splitType = this.expenseForm.get('splitType')?.value;
        const amount = this.expenseForm.get('amount')?.value;

        const selectedUsers = this.groupMembers.filter((m: GroupMember) => this.selectedMemberIds.includes(m.userId));
        
        return selectedUsers.map(m => ({
            userId: m.userId,
            shareAmount: splitType === 3 ? this.exactAmounts[m.userId] || 0 : null,
            sharePercentage: splitType === 2 ? this.percentages[m.userId] || 0 : null
        }));
    }

    getEqualShare(): number {
        const amount = Number(this.expenseForm.get('amount')?.value || 0);
        const count = this.selectedMemberIds.length;

        return count > 0 ? Math.round((amount / count) * 100) / 100 : 0;
    }

    getTotalPercentages(): number {
        return this.selectedMemberIds.reduce((sum, id) => sum + (this.percentages[id] || 0), 0);
    }

    getTotalExact(): number {
        return this.selectedMemberIds.reduce((sum, id) => sum + (this.exactAmounts[id] || 0), 0);
    }

    toggleMember(userId: string, checked: boolean): void {
        if(checked)
        {
            if(!this.selectedMemberIds.includes(userId))
            {
                this.selectedMemberIds.push(userId);
            }
        }
        else
        {
            this.selectedMemberIds = this.selectedMemberIds.filter(id => id !== userId);
        }
    }

    get title() { return this.expenseForm.get('title'); }
    get amount() { return this.expenseForm.get('amount'); }
    get groupIdControl() { return this.expenseForm.get('groupId'); }
    get currentSplitType(): number {
        return this.expenseForm.get('splitType')?.value;
    }
}