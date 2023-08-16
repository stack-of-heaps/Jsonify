<template>
    <div>
        <div v-for="(jsonifyPropertyArray, index) in props.jsonifyProperty.collections" :key="index">
            <div v-for="(jsonifyProperty, index) in jsonifyPropertyArray" :key="'jsonProp' + index">
                <DataDisplay :jsonifyProperty="jsonifyProperty"/>
            </div>
        </div>
    </div>
</template>

<script setup lang="ts">
import {watch} from 'vue'
import { JsonifyProperty } from '../lib/typeDefinitions'
import DataDisplay from './DataDisplay.vue'
import 'element-plus/es/components/message/style/css'

interface Props {
    jsonifyProperty: JsonifyProperty
}

const props = defineProps<Props>()

watch(() => props.jsonifyProperty.arraySize, (newValue: number, oldValue: number) => {
    if (newValue > oldValue){
        let propertiesJson = JSON.stringify(props.jsonifyProperty.properties)
        let propertiesObject = JSON.parse(propertiesJson)
        props.jsonifyProperty.collections[newValue] = propertiesObject
    }
    if (newValue < oldValue){
        delete props.jsonifyProperty.collections[oldValue]
    }
})

</script>