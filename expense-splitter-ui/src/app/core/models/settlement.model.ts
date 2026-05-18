export interface Settlement {
    id: string;
    groupId: string;
    fromUserName: string;
    toUserName: string;
    amount: number;
    status: string;
    note?: string;
    createdAt: Date;
    settledAt?: Date;
}

export interface CreateSettlementRequest {
    groupId: string;
    fromUserId: string;
    toUserId: string;
    amount: number;
    note?: string;
}
