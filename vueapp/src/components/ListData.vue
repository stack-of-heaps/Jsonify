<template>
    <div>
        <p>Number of items: {{ props.classData.arraySize }}</p>
    </div>
    <div>
        <div v-for="(jsonifyPropertyArray, index) in props.classData.collections" :key="index">
            <div v-for="(jsonifyProperty, index) in jsonifyPropertyArray" :key="'jsonProp' + index">
                <DataDisplay :classData="jsonifyProperty"/>
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
    classData: JsonifyProperty
}

const props = defineProps<Props>()

watch(() => props.classData.arraySize, (newValue: number, oldValue: number) => {
    if (newValue > oldValue){
        let propertiesJson = JSON.stringify(props.classData.properties)
        let propertiesObject = JSON.parse(propertiesJson)
        props.classData.collections[newValue] = propertiesObject
    }
    if (newValue < oldValue){
        delete props.classData.collections[oldValue]
    }
})

</script>