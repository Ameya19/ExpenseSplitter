import { User } from "./user.model";

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
    role: number;
    isAdmin: boolean;
    user: User
}

export interface CreateGroupRequest {
    name: string;
    description?: string;
    createdByUserId: string;
}

export interface MemberRole {
    userId: string;
    groupId: string
    role: string;
    isAdmin: boolean;
}