<script setup lang="ts">
import { JsonifyProperty, PropertyTypes } from '../lib/typeDefinitions'

interface Props {
    jsonifyProperty: JsonifyProperty
}
const props = defineProps<Props>()

function getObject(properties: JsonifyProperty[]): Record<string, any> {
    if (!properties){
        return null!;
    }
    let newObject: Record<string, any> = {}
    properties.forEach(property => {
        newObject[property.displayName] = getDefaultValue(property)
    })

    return newObject
}

function getDefaultValue(property: JsonifyProperty): any {
    if (property.propertyType === PropertyTypes.Object)
        return getObject(property.properties)
    else if (property.propertyType === PropertyTypes.List) {
        if (!property.collections)
            return null!

        let newArray = []
        for (let key in property.collections){
            newArray.push(getObject(property.collections[key]))
        }
        return newArray
    }
    else
        return property.setValue
}

function jsonifyProps(jsonifyProperty: JsonifyProperty): string {
    return JSON.stringify(getDefaultValue(jsonifyProperty), null, 2) 
}

function copyToClipboard(){
    let jsonString = jsonifyProps(props.jsonifyProperty)
    navigator.clipboard.writeText(jsonString);
}

</script>

<style>
.jsonViewText{
    color: #f8f8f2;
}
pre {
    color: #f8f8f2;
    font-size: 14px;
}
</style>

<template>
    <el-button size="small" @click="copyToClipboard">Copy to Clipboard</el-button>
    <p class="jsonViewText"><strong>{{ props.jsonifyProperty.displayName }}</strong></p>
    <pre>{{ jsonifyProps(props.jsonifyProperty) }}</pre>
</template>