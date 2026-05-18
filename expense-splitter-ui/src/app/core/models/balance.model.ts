export interface Balance {
    userId: string;
    displayName: string;
    netBalance: number;
    status: string;
}

export interface SettlementSuggestion {
    fromUserId: string;
    fromUserName: string;
    toUserId: string;
    toUserName: string;
    amount: number;
}