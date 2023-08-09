<script setup lang="ts">
import { ref, computed } from 'vue';
import type {Ref} from 'vue';
import ListData from './ListData.vue'
import { JsonifyProperty, PropertyTypes } from '../lib/typeDefinitions'
import 'element-plus/es/components/message/style/css'

interface Props {
    classData: JsonifyProperty
}
const props = defineProps<Props>()
// const currentPropertyValue: Ref<string | number> = ref("default")
const computedColor = computed(() => {

switch (props.classData.depth){
    case 0: return '#F56C6C';
    case 1: return '#f89898';
    case 2: return '#fab6b6';
    case 3: return '#fcd3d3';
    case 4: return '#fde2e2';
    case 5: return '#fef0f0';
    default: return '#000000';
    }
})

</script>

<style lang="scss">
.el-row {
  margin-bottom: 5px;
}

.card-header {
  display: flex;
  justify-content: space-between;
  margin: 0;
  padding: 0;
}

.testDiv {
    height: 80px;
}

.text {
  font-size: 14px;
  color: black;
}

.item {
  margin-bottom: 18px;
}

.box-card {
  max-width: 500px;
}
</style>

<template>
    <div v-if="props.classData?.displayName">
        <el-card :body-style="{ padding: '10px' }" class="box-card" :style="{'background-color': computedColor}">
            <template #header>
                <div align="left" class="card-header">
                    <el-text class="text"><b>{{ props.classData.displayName }}</b></el-text>
                    <div v-if="props.classData.propertyType === PropertyTypes.List">
                        <el-input-number size="small" />
                    </div>
                </div>
            </template>
            <div v-if="props.classData.propertyType === PropertyTypes.Boolean" align="left"> 
                    <el-switch 
                    v-model="props.classData.setValue"
                    size="small"
                    active-text="True"
                    inactive-text="False"
                    />
            </div>
            <div v-if="props.classData.propertyType === PropertyTypes.DateTime" align="left"> 
                <div class="block">
                    <el-date-picker
                        v-model="props.classData.setValue"
                        type="datetime"
                        placeholder="Select date and time"
                    />
                </div>
            </div>
            <div v-if="props.classData.propertyType === PropertyTypes.Decimal"> 
                    <el-input v-model="props.classData.setValue" size="small" />
            </div>
            <div v-if="props.classData.propertyType === PropertyTypes.Enum" align="left"> 
                    <el-select v-model="props.classData.setValue" filterable clearable class="m-2" placeholder="Choose One" size="small">
                    <el-option v-for="(enumValue, index) in props.classData.enumeratedProperties" size="small"
                                :key="index"
                                :label="enumValue"
                                :value="enumValue" />
                    </el-select> 
            </div>
            <div v-if="props.classData.propertyType === PropertyTypes.Integer"> 
                    <el-input v-model="props.classData.setValue" size="small" />
            </div>

            <div v-if="props.classData.propertyType === PropertyTypes.List">
                <ListData :classData="props.classData" />
            </div>
            
            <div v-if="props.classData.propertyType === PropertyTypes.Object"> 
                    <div v-for="(classProperty, index) in props.classData.properties" :key="index">
                        <DataDisplay :classData="classProperty"/>
                    </div>
            </div>

            <div v-if="props.classData.propertyType === PropertyTypes.String"> 
                        <el-input v-model="props.classData.setValue" placeholder="placeholder" size="small"/>
            </div>
        </el-card>
    </div>
</template>
