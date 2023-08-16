<script setup lang="ts">
import { ref } from 'vue';
import { DotNetProperty, JsonifyProperty, PropertyTypes } from '../lib/typeDefinitions'
import JsonView from './JsonView.vue'
import DataDisplay from './DataDisplay.vue'
import 'element-plus/es/components/message/style/css'

interface Props {
    property: DotNetProperty,
    propertyProvided: Boolean
}

const props = defineProps<Props>()
const mappedProperty = ref(mapToJsonifyProperty(props.property))

function getDefaultValue(property: DotNetProperty): any {
    switch(property.propertyType){
        case PropertyTypes.Boolean:
            if (property.displayName === "IsCitizen")
                return true;
            return false;

        case PropertyTypes.DateTime:
            return new Date().toISOString();

        case PropertyTypes.Decimal:
            return 0.0;

        // For Country, set default value to USA.
        // For State, set default value to NE.
        case PropertyTypes.Enum:
            // This should never happen. But just in case.
            if (!property.enumeratedProperties){
                return null;
            }

            let usaIndex = property.enumeratedProperties.findIndex(enumVal => enumVal === "USA");
            if (usaIndex !== -1){
                return property.enumeratedProperties[usaIndex]
            }

            let neIndex = property.enumeratedProperties.findIndex(enumVal => enumVal === "NE");
            if (neIndex !== -1){
                return property.enumeratedProperties[neIndex]
            }

            let purposeOfInsuranceIndex = property.enumeratedProperties.findIndex(enumVal => enumVal === "PurposeOfInsurance")
            if (purposeOfInsuranceIndex !== -1){
                return property.enumeratedProperties[purposeOfInsuranceIndex]
            }

            return property.enumeratedProperties[0]

        case PropertyTypes.Integer:
            switch (property.displayName){
                case "Feet":
                    return 5;
                case "Inches":
                    return 10;
                case "WeightInPounds":
                    return 175;
                default:
                    return 0;
            }

        case PropertyTypes.Object:
            {
                if (property.displayName === "IndividualInterview"){
                    return null;
                }

                let newObject: Record<string, any> = {}
                property.properties.forEach(property => {
                    newObject[property.displayName] = getDefaultValue(property)
                })

                return newObject
            }

        case PropertyTypes.String:
            switch (property.displayName){
                case "EmailAddress": 
                    return "exampleEmail@email.com";
                case "AreaCode":
                    return "402";
                case "Number":
                    return "555-2342";
                case "ZipCode":
                    return "68506";
                default:
                    return property.displayName;
            }
    }
}

function mapToJsonifyProperty(property: DotNetProperty): JsonifyProperty {
    let mappedProperties = [] as JsonifyProperty[]
    if (property.properties){
        mappedProperties = property.properties.map(property => mapToJsonifyProperty(property))
    }
    
    let jsonifyProperty = {
        displayName: property.displayName,
        depth: property.depth,
        enumeratedProperties: property.enumeratedProperties ?? [],
        properties: mappedProperties,
        propertyType: property.propertyType,
        setValue: getDefaultValue(property),
        arraySize: 1
    } as JsonifyProperty

    if (property.propertyType === PropertyTypes.List){
        jsonifyProperty.collections = { 1: mappedProperties };
    }

    return jsonifyProperty;
}
</script>

<style>
    .box{
        display: flex;
        align-items: center;
        justify-content: space-evenly;
        gap: 50px
    }

    .dataDisplay {
        align-self: flex-start;
        width: 100%;
        height: 100%;
        flex-basis: auto;
    }

    .jsonDisplay {
        width: 100%;
        align-self: flex-start;
        flex-basis: auto;
    }
</style>

<template>
    <div v-if="propertyProvided">
        <div class="box">
            <div class="dataDisplay">
                <DataDisplay :jsonifyProperty="mappedProperty" />
            </div>
            <div class="jsonDisplay">
                <JsonView :jsonify-property="mappedProperty" />
            </div>
        </div>
    </div>
</template>