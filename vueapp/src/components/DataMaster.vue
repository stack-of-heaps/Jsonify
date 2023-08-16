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
            return false;

        case PropertyTypes.DateTime:
            return new Date().toISOString();

        case PropertyTypes.Decimal:
            return 0.0;

        case PropertyTypes.Enum:
            // This should never happen. But just in case.
            if (!property.enumeratedProperties){
                return null;
            }

            return property.enumeratedProperties[0]

        case PropertyTypes.Integer:
            return 0;

        case PropertyTypes.Object:
            {
                let newObject: Record<string, any> = {}
                property.properties.forEach(property => {
                    newObject[property.displayName] = getDefaultValue(property)
                })

                return newObject
            }

        case PropertyTypes.String:
            return property.displayName;

        default:
            return "abc";
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
        justify-content: center;
        align-items: space-evenly;
        gap: 50px;
        height: 100%;
        width: 100%;
    }

    .jsonDisplay {
        align-self: flex-start;
    }
</style>

<template>
    <div v-if="propertyProvided">
        <div class="box">
            <div>
                <DataDisplay :jsonifyProperty="mappedProperty" />
            </div>
            <div class="jsonDisplay">
                <JsonView :jsonify-property="mappedProperty" />
            </div>
        </div>
    </div>
    <div v-else>No data yet.</div>
</template>