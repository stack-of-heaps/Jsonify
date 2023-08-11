<script setup lang="ts">
import { ref } from 'vue';
import { Property, JsonifyProperty, PropertyTypes } from '../lib/typeDefinitions'
import JsonView from './JsonView.vue'
import DataDisplay from './DataDisplay.vue'
import 'element-plus/es/components/message/style/css'

interface Props {
    property: Property,
    propertyProvided: Boolean
}

const props = defineProps<Props>()
const mappedProperty = ref(mapToJsonifyProperty(props.property))

function getObject(properties: Property[]): Record<string, any> {
    let newObject: Record<string, any> = {}
    properties.forEach(property => {
        newObject[property.displayName] = getDefaultValue(property)
    })


    return newObject
}

function getDefaultValue(property: Property): any {
    switch(property.propertyType){
        case PropertyTypes.Boolean:
            return false;

        case PropertyTypes.DateTime:
            return new Date().toISOString();

        case PropertyTypes.Decimal:
            return 0.0;

        case PropertyTypes.Enum:
            if (!property.enumeratedProperties){
                return null;
            }

            return property.enumeratedProperties[0]

        case PropertyTypes.Integer:
            return 0;

        case PropertyTypes.List:{
            let newArray = []
            newArray.push(getObject(property.properties));
            return newArray;
        }

        case PropertyTypes.Object:
            return getObject(property.properties)

        case PropertyTypes.String:
            return "abc";

        default:
            return "abc";
    }
}

function mapToJsonifyProperty(property: Property): JsonifyProperty {
    
    return {
        displayName: property.displayName,
        depth: property.depth,
        enumeratedProperties: property.enumeratedProperties ?? [],
        nullable: property.nullable,
        properties: mapToJsonifyProperties(property.properties),
        propertyType: property.propertyType,
        setValue: getDefaultValue(property),
        arraySize: 1
    }
}

function mapToJsonifyProperties(properties: Property[]): JsonifyProperty[] | [] {
    if (!properties){
        return []
    }

    console.log("PROPERTIES: ", properties)

    return properties.map(property => mapToJsonifyProperty(property))
}

</script>

<style>
    .box{
        display: flex;
        align-items: center;
        justify-content: space-evenly;
        gap: 50px
    }
</style>

<template>
    <div v-if="propertyProvided">
        <div class="box">
            <div>
                <DataDisplay :classData="mappedProperty" />
            </div>
            <div>
                <JsonView :jsonify-property="mappedProperty" />
            </div>
        </div>
    </div>
    <div v-else>No data yet.</div>
</template>