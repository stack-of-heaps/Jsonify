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

// Some default values are set purposely to satisfy common
// validation requirements.
// For example, most areas where we feature ZipCode require
// a specific format/length.
// Another example: When we encounter Country, set to USA, as this
// is by far the most used value across contexts and services.
function getDefaultValue(property: DotNetProperty): any {
    switch(property.propertyType){
        case PropertyTypes.Boolean:
            return property.displayName === "IsCitizen" 
                ? true
                : false;

        case PropertyTypes.DateTime:
            return new Date().toISOString();

        case PropertyTypes.Decimal:
            return 0.0;

        // For Country, set default value to USA.
        // For State, set default value to NE.
        case PropertyTypes.Enum:
            {
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

                let relationshipToPrimaryInsuredIndex = property.enumeratedProperties.findIndex(enumVal => enumVal === "Estate")
                if (relationshipToPrimaryInsuredIndex !== -1){
                    return property.enumeratedProperties[relationshipToPrimaryInsuredIndex]
                }

                return property.enumeratedProperties[0]
            }

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

// Maps a DotNetProperty to a JsonifyProperty.
// The output of this function is passed to the display elements.
// ArraySize, properties, collections, setValue are all subject to mutation.
// This is against "general bad practices" but in our case the app 
// is designed around mutating props.
// ArraySize:   When this changes, JsonifyProperty.properties are copied into
//              collections. This forms the new collection.
// Collections: See note above.
// Properties:  Indirectly mutated (arraySize, collections, setValue)
// SetValue:    For everything other than Lists, this is the value which
//              is displayed/modified.
// We store a "backup" of SetValue in DefaultValue to support null functionality.
// If you make a property null but want it back, we use the DefaultValue to restore.
function mapToJsonifyProperty(property: DotNetProperty): JsonifyProperty {
    let mappedProperties = [] as JsonifyProperty[]
    if (property.properties){
        mappedProperties = property.properties.map(property => mapToJsonifyProperty(property))
    }

    let defaultValue = getDefaultValue(property)
    
    let jsonifyProperty = {
        arraySize: 1,
        defaultValue: defaultValue,
        depth: property.depth,
        displayName: property.displayName,
        enumeratedProperties: property.enumeratedProperties ?? [],
        properties: mappedProperties,
        propertyType: property.propertyType,
        setValue: defaultValue
    } as JsonifyProperty

    if (property.propertyType === PropertyTypes.List){
        let collections = { 1: mappedProperties };
        jsonifyProperty.collections = collections;
        jsonifyProperty.defaultValue = collections;
    }
    
    if (property.propertyType === PropertyTypes.Object){
        jsonifyProperty.defaultValue = jsonifyProperty.properties
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
        width: 70%;
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