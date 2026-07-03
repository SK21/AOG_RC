/*******************************************************************************
** @file        AgIsoStack.hpp
** @author      Automatic Code Generation
** @date        October 19, 2024 at 23:45:04
** @brief       Includes all important files in the AgIsoStack library.
**
** Copyright 2024 The AgIsoStack++ Developers
*******************************************************************************/

#ifndef AG_ISO_STACK_HPP
#define AG_ISO_STACK_HPP

#include "can_hardware_plugin.hpp"
#include "flex_can_t4_plugin.hpp"
#include "can_callbacks.hpp"
#include "isobus_language_command_interface.hpp"
#include "isobus_virtual_terminal_client.hpp"
#include "can_NAME.hpp"
#include "event_dispatcher.hpp"
#include "can_partnered_control_function.hpp"
#include "isobus_time_date_interface.hpp"
#include "circular_buffer.hpp"
#include "can_network_configuration.hpp"
#include "can_network_manager.hpp"
#include "isobus_maintain_power_interface.hpp"
#include "can_message_frame.hpp"
#include "nmea2000_message_definitions.hpp"
#include "can_control_function.hpp"
#include "can_general_parameter_group_numbers.hpp"
#include "isobus_shortcut_button_interface.hpp"
#include "kinetis_flexcan.hpp"
#include "isobus_guidance_interface.hpp"
#include "isobus_functionalities.hpp"
#include "isobus_task_controller_client_objects.hpp"
#include "isobus_device_descriptor_object_pool.hpp"
#include "nmea2000_message_interface.hpp"
#include "isobus_virtual_terminal_objects.hpp"
#include "can_internal_control_function.hpp"
#include "isobus_virtual_terminal_base.hpp"
#include "isobus_data_dictionary.hpp"
#include "can_message_data.hpp"
#include "isobus_standard_data_description_indices.hpp"
#include "isobus_virtual_terminal_client_state_tracker.hpp"
#include "thread_synchronization.hpp"
#include "isobus_task_controller_client.hpp"
#include "processing_flags.hpp"
#include "nmea2000_fast_packet_protocol.hpp"
#include "isobus_preferred_addresses.hpp"
#include "to_string.hpp"
#include "can_parameter_group_number_request_protocol.hpp"
#include "platform_endianness.hpp"
#include "can_extended_transport_protocol.hpp"
#include "imxrt_flexcan.hpp"
#include "can_transport_protocol.hpp"
#include "can_transport_protocol_base.hpp"
#include "can_NAME_filter.hpp"
#include "data_span.hpp"
#include "can_identifier.hpp"
#include "FlexCAN_T4.hpp"
#include "isobus_heartbeat.hpp"
#include "isobus_virtual_terminal_client_update_helper.hpp"
#include "can_hardware_abstraction.hpp"
#include "system_timing.hpp"
#include "can_constants.hpp"
#include "can_badge.hpp"
#include "can_stack_logger.hpp"
#include "can_message.hpp"
#include "isobus_speed_distance_messages.hpp"
#include "isobus_diagnostic_protocol.hpp"
#include "can_hardware_interface_single_thread.hpp"

#endif // AG_ISO_STACK_HPP
