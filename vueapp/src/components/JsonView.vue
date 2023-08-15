<script setup lang="ts">
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
    else if (property.propertyType === PropertyTypes.List){
        let newArray = []
        for (let key in property.collections){
            newArray.push(getObject(property.collections[key]))
        }
        return newArray
    }
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