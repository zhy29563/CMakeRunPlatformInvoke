cmake_minimum_required(VERSION 3.18)
project(CMakeRunPlatformInvoke LANGUAGES CSharp CXX)

SET(CMAKE_BUILD_TYPE "Debug")

# 指定C/C++标准
set(CMAKE_CXX_STANDARD 14)
set(CMAKE_CXX_STANDARD_REQUIRED True)
set(CMAKE_C_STANDARD 14)
set(CMAKE_C_STANDARD_REQUIRED True)

# 设定输出目录
set(CMAKE_RUNTIME_OUTPUT_DIRECTORY_DEBUG  ${PROJECT_SOURCE_DIR}/build/Debug)
set(CMAKE_ARCHIVE_OUTPUT_DIRECTORY_DEBUG  ${PROJECT_SOURCE_DIR}/build/Debug)
set(CMAKE_RUNTIME_OUTPUT_DIRECTORY_RLEASE ${PROJECT_SOURCE_DIR}/build/Release)
set(CMAKE_ARCHIVE_OUTPUT_DIRECTORY_RLEASE ${PROJECT_SOURCE_DIR}/build/Release)
##############################################################################
# 增加子目录
add_subdirectory(NativeLib)

##############################################################################
# CS测试
add_executable(HelloWorldWithCustomedFunction   "sources/HelloWorldWithCustomedFunction.cs")
add_executable(HelloWorldWithStandardCRuntime   "sources/HelloWorldWithStandardCRuntime.cs")
add_executable(HelloWorldWithWin32              "sources/HelloWorldWithWin32.cs")

add_executable(DllImportAttributeSample         "sources/DllImportAttributeSample.cs")

##############################################################################
# 项目配置
set(target_names
    HelloWorldWithCustomedFunction
    HelloWorldWithStandardCRuntime
    HelloWorldWithWin32
    DllImportAttributeSample)

foreach(target_name ${target_names})
target_compile_options(${target_name} PRIVATE "-langversion:latest")
target_compile_options(${target_name} PRIVATE "-platform:x64")
set_target_properties(${target_name} PROPERTIES VS_DOTNET_TARGET_FRAMEWORK_VERSION "v4.8")
set_target_properties(${target_name} PROPERTIES VS_DOTNET_REFERENCES "Microsoft.CSharp;
                                                                      System;
                                                                      System.Core;
                                                                      mscorlib")
target_link_libraries(${target_name} PRIVATE NativeLib)
endforeach()