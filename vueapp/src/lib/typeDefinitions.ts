// Used to populate dropdown boxes.
export type ClassInfo = {
    displayName: string,
    fullName: string,
    namespace: string,
    product: string
}

// Payload from .NET Service.
export type DotNetProperty = {
    displayName: string,
    depth: number,
    enumeratedProperties: string[] | null,
    properties: DotNetProperty[],
    propertyType: PropertyTypes
}

// Derived from DotNetProperty.
export type JsonifyProperty = {
    arraySize: number,
    collections: Record<number, JsonifyProperty[]>,
    defaultValue: any,
    displayName: string,
    depth: number,
    enumeratedProperties?: string[],
    properties: JsonifyProperty[],
    propertyType: PropertyTypes,
    setValue: any
}

// Needs to correspond with .NET Solution PropertyTypes.
export enum PropertyTypes {
    Boolean = "Boolean",
    DateTime = "DateTime",
    Decimal = "Decimal",
    Enum = "Enum",
    Integer = "Integer",
    List = "List",
    Object = "Object",
    String = "String"
}

export enum ProductNames {
    None = "",
    AccidentGoToMarket = "AccidentGoToMarket",
    AccidentFlex = "AccidentFlex",
    AccidentalDeath = "AccidentalDeath",
    CenturyPlusDisabilityIncome = "CenturyPlusDisabilityIncome",
    CriticalIllness = "CriticalIllness",
    CriticalIllnessDirect = "CriticalIllnessDirect",
    IncomeProtection = "IncomeProtection",
    TermLife = "TermLife",
    TermLifeDeveloperEdition = "TermLifeDeveloperEdition"
}

export enum ServiceNames {
    None = "",
    Api = "Api",
    Forms = "Forms",
    NBFrameworkRestAPI = "NBFrameworkRestAPI",
    Occupation = "Occupation",
    Questions = "Questions",
    Quote = "Quote",
    Underwriting = "Underwriting"
}