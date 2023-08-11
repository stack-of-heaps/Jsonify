<template>
    <div>
        <p>Number of items: {{ props.classData.arraySize }}</p>
    </div>
    <div>
        <div v-for="number in props.classData.arraySize" v-bind:key="number">
            <div v-for="(classProperty, index) in props.classData.properties" :key="classProperty.displayName + index">
                <DataDisplay :classData="classProperty"/>
            </div>
        </div>
    </div>
</template>

<script setup lang="ts">
import { JsonifyProperty } from '../lib/typeDefinitions'
import DataDisplay from './DataDisplay.vue'
import 'element-plus/es/components/message/style/css'

interface Props {
    classData: JsonifyProperty
}

const props = defineProps<Props>()

console.log("LISADATA RE-RENDERING")
</script>