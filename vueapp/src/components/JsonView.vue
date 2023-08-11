<script setup lang="ts">
import { defineProps } from 'vue';
import { JsonifyProperty, PropertyTypes } from '../lib/typeDefinitions'

interface Props {
    jsonifyProperty: JsonifyProperty
}
const props = defineProps<Props>()

function getObject(properties: JsonifyProperty[]): Record<string, any> {
    let newObject: Record<string, any> = {}
    properties.forEach(property => {
        newObject[property.displayName] = getDefaultValue(property)
    })

    return newObject
}

function getDefaultValue(property: JsonifyProperty): any {
    if (property.propertyType === PropertyTypes.Object)
        return getObject(property.properties)
    else
        return property.setValue
}
</script>

<template>
    <div>
        <p><h1>{{ props.jsonifyProperty.displayName }}</h1></p>
        <pre>{{ JSON.stringify(getDefaultValue(props.jsonifyProperty), null, 2) }}</pre>
    </div>
</template>