export interface Group {
    id: string;
    name: string;
    description?: string;
    createdByUserId: string;
    createdAt: Date;
    memberCount: number;
}

export interface GroupDetail extends Group {
    members: GroupMember[];
    expenseCount: number;
}

export interface GroupMember {
    userId: string;
    displayName: string;
    joinedAt: Date;
    role: string;
}

export interface CreateGroupRequest {
    name: string;
    description?: string;
    createdByUserId: string;
}