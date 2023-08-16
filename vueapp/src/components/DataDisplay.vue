<script setup lang="ts">
import { computed } from 'vue';
import ListData from './ListData.vue'
import { JsonifyProperty, PropertyTypes } from '../lib/typeDefinitions'
import 'element-plus/es/components/message/style/css'

interface Props {
    jsonifyProperty: JsonifyProperty
}

const props = defineProps<Props>()
const computedColor = computed(() => {

switch (props.jsonifyProperty.depth){
    case 0: return '#4d908e';
    case 1: return '#43aa8b';
    case 2: return '#90be6d';
    case 3: return '#FACD62';
    case 4: return '#f8961e';
    case 5: return '#f3722c';
    case 6: return '#FB7E80';
    default: return '#277da1';
    }
})
</script>

<style lang="scss">
.card-header {
  display: flex;
  justify-content: space-between;
  margin: 0;
  padding: 0;
}

.text {
  font-size: 18px;
  color: #1f363d;
}

.propertyType {
    font-size: 16px;
    font-style: italic;
    font-weight: 800;
    color: #F2F7EF;
}
</style>

<template>
    <div v-if="props.jsonifyProperty?.displayName">
        <el-card class="box-card" :style="{'background-color': computedColor}">
            <template #header>
                <div align="left" class="card-header">
                    <el-text class="text"><b>{{ props.jsonifyProperty.displayName }}</b></el-text>
                    <el-text class="propertyType"> {{ props.jsonifyProperty.propertyType }}</el-text>
                    <div v-if="props.jsonifyProperty.propertyType === PropertyTypes.List">
                        <el-input-number size="small" v-model="props.jsonifyProperty.arraySize" />
                    </div>
                </div>
            </template>
            <div v-if="props.jsonifyProperty.propertyType === PropertyTypes.Boolean" align="left"> 
                <el-switch 
                v-model="props.jsonifyProperty.setValue"
                style="--el-switch-on-color: #41F9F6; --el-switch-off-color: #f94144"
                size="small"
                active-text="True"
                inactive-text="False"
                />
            </div>
            <div v-if="props.jsonifyProperty.propertyType === PropertyTypes.DateTime" align="left"> 
                <div class="block">
                    <el-date-picker
                        v-model="props.jsonifyProperty.setValue"
                        type="datetime"
                        placeholder="Select date and time"
                    />
                </div>
            </div>
            <div v-if="props.jsonifyProperty.propertyType === PropertyTypes.Decimal"> 
                    <el-input v-model="props.jsonifyProperty.setValue" size="small" />
            </div>
            <div v-if="props.jsonifyProperty.propertyType === PropertyTypes.Enum" align="left"> 
                <el-select v-model="props.jsonifyProperty.setValue" filterable clearable class="m-2" placeholder="Choose One" size="small">
                <el-option v-for="(enumValue, index) in props.jsonifyProperty.enumeratedProperties" size="small"
                            :key="index"
                            :label="enumValue"
                            :value="enumValue" />
                </el-select> 
            </div>
            <div v-if="props.jsonifyProperty.propertyType === PropertyTypes.Integer"> 
                <el-input v-model="props.jsonifyProperty.setValue" size="small" />
            </div>

            <div v-if="props.jsonifyProperty.propertyType === PropertyTypes.List">
                <ListData :jsonifyProperty="props.jsonifyProperty" />
            </div>
            
            <div v-if="props.jsonifyProperty.propertyType === PropertyTypes.Object"> 
                <div v-for="(classProperty, index) in props.jsonifyProperty.properties" :key="index">
                    <DataDisplay :jsonifyProperty="classProperty"/>
                </div>
            </div>

            <div v-if="props.jsonifyProperty.propertyType === PropertyTypes.String"> 
                <el-input v-model="props.jsonifyProperty.setValue" placeholder="placeholder" size="small"/>
            </div>
        </el-card>
    </div>
</template>