export interface GroupReport{
    groupId: string,
    groupName: string,
    totalAmount: number,
    totalTransactions: number,
    categoryBreakdown: CategoryBreakdown[],
    memberSpending: MemberSpending[],
    monthlyBreakdown: MonthlyBreakdown[]
}

export interface CategoryBreakdown{
    category: string,
    totalAmount: number,
    count: number,
    percentage: number
}

export interface MemberSpending{
    userId: string,
    displayName: string,
    totalPaid: number,
    totalOwed: number,
    netBalance: number
}

export interface MonthlyBreakdown{
    year: number,
    month: number,
    monthName: string,
    totalAmount: number,
    totalTransactions: number
}

export interface UserReport{
    userId: string,
    displayName: string,
    totalPaidAcrossGroups: number,
    totalOwedAcrossGroups: number,
    netBalanceAcrossGroups: number,
    groupSummaries: GroupSummary[],
    categoryBreakdown: CategoryBreakdown[]
}

export interface GroupSummary{
    groupId: string,
    groupName: string,
    totalPaid: number,
    netBalance: number
}