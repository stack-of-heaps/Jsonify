export type ClassInfo = {
    displayName: string,
    fullName: string,
    namespace: string,
    product: string,
    version: string
}

export type Property = {
    displayName: string,
    depth: number,
    enumeratedProperties: string[] | null,
    nullable: boolean,
    properties: Property[] | null,
    type: string,
    propertyType: PropertyTypes,
    setValue: any
}

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