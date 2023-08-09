<script setup lang="ts">
import { defineProps } from 'vue';
import { JsonifyProperty, PropertyTypes } from '../lib/typeDefinitions'

interface Props {
    jsonifyProperty: JsonifyProperty
}
const props = defineProps<Props>()

function getProperty(property: JsonifyProperty, tempObject: Record<string, any>): any {
    /*
    if (property.propertyType !== PropertyTypes.Object){
        tempObject[property.displayName] = property.setValue;
    }
    else {
        tempObject[property.displayName] = getObject(property.properties)
    }
    return tempObject;
    */

    // Code above produces circular recursion error.
    // Code below works correctly on initial render but updates
    // to nested objects are not working.
    tempObject[property.displayName] = property.setValue;
    return tempObject;
}

function getObject(properties: JsonifyProperty[]): object{
    let newObject: Record<string, any> = {}
    properties.forEach(property => {
        newObject[property.displayName] = getProperty(property, newObject)
    })

    return newObject
}

function seed(input: JsonifyProperty) : string {
    let propertiesObject = {}
    props.jsonifyProperty.properties?.map(property => getProperty(property, propertiesObject))
    let jsonObject = { [props.jsonifyProperty.displayName] : propertiesObject }

    return JSON.stringify(jsonObject)
}

</script>

<template>
    <div>Json Output</div>
    <div>{{ seed($props.jsonifyProperty) }}</div>
</template>