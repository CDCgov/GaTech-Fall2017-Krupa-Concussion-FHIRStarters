export class Patient {
    public data: Observations;
    public age: IAgeProperty;

    constructor() {
        this.data = new Observations();

        // data seeding TODO: remove this
        this.age = { value: new Age(null, AgeUnit.Years), source: DataSource.Manual };
        this.data.GlasgowComaScale = { value: null, source: DataSource.Manual };
        this.data.LossOfConsciousness = { value: TriState.Unknown, source: DataSource.Manual };
        this.data.LossOfConsciousnessTime = { value: new Timespan(null, TimeUnit.Minutes), source: DataSource.Manual };
        this.data.ConcussionHistory = { value: TriState.Unknown, source: DataSource.Manual };
        this.data.Vomiting = { value: TriState.Unknown, source: DataSource.Manual };
        this.data.SignsOfPalpableSkullFracture = { value: TriState.Unknown, source: DataSource.Manual };
        this.data.SignsOfAlteredMentalStatus = { value: TriState.Unknown, source: DataSource.Manual };
        this.data.OptScalpHematoma = { value: TriState.Unknown, source: DataSource.Manual };
        this.data.SevereMechanismOfInjury = { value: TriState.Unknown, source: DataSource.Manual };
        this.data.AbnormalBehaviorPerParentalAssessment = { value: TriState.Unknown, source: DataSource.Manual };
        this.data.SignsOfBasilarSkullFracture = { value: TriState.Unknown, source: DataSource.Manual };
        this.data.AnyHeadaches = { value: TriState.Unknown, source: DataSource.Manual };
        this.data.SevereHeadache = { value: TriState.Unknown, source: DataSource.Manual };
        this.data.WorseningSymptoms = { value: TriState.Unknown, source: DataSource.Manual };
        this.data.CtImagingTaken = { value: TriState.Unknown, source: DataSource.Manual };
        this.data.CtImagingResultsAbnormal = { value: TriState.Unknown, source: DataSource.Manual };
        this.data.LightNoiseSensitivity = { value: TriState.Unknown, source: DataSource.Manual };
        this.data.MinorBluntHeadTraumaContusions = { value: TriState.Unknown, source: DataSource.Manual };
        this.data.MinorBluntHeadTraumaContusionsSmallAndIsolated = { value: TriState.Unknown, source: DataSource.Manual };
        this.data.RestRecommended = { value: false, source: DataSource.Manual };
        this.data.RestRecommendedDays = { value: null, source: DataSource.Manual };
        this.data.IncludeSchoolRecommendations = { value: false, source: DataSource.Manual };
        this.data.SignsOfOtherSkullFracture = { value: TriState.Unknown, source: DataSource.Manual };
        this.data.OtherScalpHematoma = { value: TriState.Unknown, source: DataSource.Manual };
        this.data.ChronicDiseases = { value: TriState.Unknown, source: DataSource.Manual };
        this.data.SevereSymptoms = { value: TriState.Unknown, source: DataSource.Manual };
        this.data.PersistentMentalStatusChanges = { value: TriState.Unknown, source: DataSource.Manual };
        this.data.CerebralContusionsSuspected = { value: TriState.Unknown, source: DataSource.Manual };
        this.data.MriTaken = { value: TriState.Unknown, source: DataSource.Manual };
        this.data.MriResultsAbnormal = { value: TriState.Unknown, source: DataSource.Manual };

        this.data.CustomDischargeInstructions = { value: null, source: DataSource.Manual };
    }

    public toJson(): string {
        const result = {
            age: this.age.value && this.age.value.value ? this.age.value : null,
            data: this.data.toJson()
        };
        return JSON.stringify(result);
    }
}

export class Observations {
    CtImagingTaken: ITriStateProperty;
    CtImagingResultsAbnormal: ITriStateProperty;
    GlasgowComaScale: INumberProperty;
    LossOfConsciousness: ITriStateProperty;
    LossOfConsciousnessTime: ITimespanProperty;
    ConcussionHistory: ITriStateProperty;
    Vomiting: ITriStateProperty;
    SignsOfPalpableSkullFracture: ITriStateProperty;
    SignsOfAlteredMentalStatus: ITriStateProperty;
    OptScalpHematoma: ITriStateProperty;
    SevereMechanismOfInjury: ITriStateProperty;
    AbnormalBehaviorPerParentalAssessment: ITriStateProperty;
    SignsOfBasilarSkullFracture: ITriStateProperty;
    AnyHeadaches: ITriStateProperty;
    SevereHeadache: ITriStateProperty;
    WorseningSymptoms: ITriStateProperty;
    LightNoiseSensitivity: ITriStateProperty;
    MinorBluntHeadTraumaContusions: ITriStateProperty;
    MinorBluntHeadTraumaContusionsSmallAndIsolated: ITriStateProperty;
    SignsOfOtherSkullFracture: ITriStateProperty;
    OtherScalpHematoma: ITriStateProperty;
    ChronicDiseases: ITriStateProperty;
    SevereSymptoms: ITriStateProperty;
    PersistentMentalStatusChanges: ITriStateProperty;
    CerebralContusionsSuspected: ITriStateProperty;
    MriTaken: ITriStateProperty;
    MriResultsAbnormal: ITriStateProperty;

    RestRecommended: IBooleanProperty;
    RestRecommendedDays: INumberProperty;
    IncludeSchoolRecommendations: IBooleanProperty;
    CustomDischargeInstructions: IStringProperty;

    public toJson(): any {
        const result: any = {};
        const observations = this;
        function isDataSourced(object: any): object is IDataSourcedProperty {
            return ("value" in object) && ("source" in object);
        }
        for (let prop in observations) {
            if (observations.hasOwnProperty(prop)) {
                result[prop] = isDataSourced(observations[prop])
                    ? (observations[prop] as IDataSourcedProperty).value
                    : observations[prop];
                if (result[prop] instanceof Timespan) {
                    result[prop] = (result[prop] as Timespan).toString();
                }
            }
        }
        return result;
    }
}

export class Timespan {
    constructor(public value: number | null, public unit: TimeUnit) {
    }

    public toString(): string | null {
        if (this.value == null) return null;
        switch(this.unit) {
            case TimeUnit.Minutes:
                return `00:${this.paddedValue()}:00`;
            case TimeUnit.Seconds:
                return `00:00:${this.paddedValue()}`;
        }
        return null;
    }

    private paddedValue(): string {
        return (`00${this.value}`).slice(-2);
    }
}

export class Age {
    constructor(public value: number | null, public unit: AgeUnit) {
    }

    public static fromBirthDate(birthDateString: string): Age {
        const today = new Date();
        const birthDate = new Date(birthDateString);
        const ageInYears = today.getFullYear() - birthDate.getFullYear();
        const m = today.getMonth() - birthDate.getMonth();
        let months = ageInYears * 12 + m;
        if (today.getDate() < birthDate.getDate()) {
            months--;
        }

        return months > 23 ? new Age(Math.floor(months / 12), AgeUnit.Years) : new Age(months, AgeUnit.Months);
    }
}

export interface IDataSourcedProperty {
    source: DataSource;
    value: any;
}

export interface IStringProperty {
    source: DataSource;
    value: string | null;
}

export interface ITimespanProperty extends IDataSourcedProperty {
    value: Timespan | null;
}

export interface IAgeProperty extends IDataSourcedProperty {
    value: Age | null;
}

export interface INumberProperty extends IDataSourcedProperty {
    value: number | null;
}

export interface ITriStateProperty extends IDataSourcedProperty {
    value: TriState;
}

export interface IBooleanProperty extends IDataSourcedProperty {
    value: boolean | null;
}

export enum TriState {
    Unknown,
    Yes,
    No
}

export enum DataSource {
    Manual,
    FHIR
}

export enum AgeUnit {
    Months,
    Years
}

export enum TimeUnit {
    Seconds,
    Minutes
}
