export interface Expense {
    id: string;
    title: string;
    amount: number;
    category: number;
    paidByUserId: string;
    groupId: string;
    date: Date;
    splitType: number;
}

export interface createExpenseRequest {
    groupId: string;
    title: string;
    amount: number;
    category: number;
    paidByUserId: string;
    date: Date;
    splitType: number;
}