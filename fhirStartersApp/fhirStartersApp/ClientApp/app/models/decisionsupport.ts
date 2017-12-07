export class DecisionSupportResult {
    public typeDescription: string;

    constructor(public ruleLabel: string, public description: string, public type: DecisionSupportResultType) {
        switch (this.type) {
            case DecisionSupportResultType.Error:
                this.typeDescription = "Error Occurred";
                break;
            case DecisionSupportResultType.NoAction:
                this.typeDescription = "No Action";
                break;
            case DecisionSupportResultType.ActionRecommendation:
                this.typeDescription = "Action Recommended";
                break;
            case DecisionSupportResultType.ManagementPlanRecommendation:
                this.typeDescription = "Management Plan Recommendation";
                break;
            case DecisionSupportResultType.MoreInformationRequired:
                this.typeDescription = "More Information Required";
                break;
            default:
                this.typeDescription = "Other";
        }
    }

    public static fromJson(json: any): DecisionSupportResult {
        return new DecisionSupportResult(json.ruleLabel, json.description, json.type);
    }
}

export enum DecisionSupportResultType {
    NoAction,
    Error,
    MoreInformationRequired,
    ActionRecommendation,
    ManagementPlanRecommendation
}